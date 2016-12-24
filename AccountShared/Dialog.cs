using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using VkNet.Model;

namespace VkData
{
    public class Dialog<TMessage>
    {
        private static readonly Dictionary<Type, int> _nodeCountDictionary = new Dictionary<Type, int>();
        private readonly int _nodeCount = 20;
        private LinkedList<List<TMessage>> _all = new LinkedList<List<TMessage>>();

        public Dialog()
        {
        }

        private Dialog(string name)
        {
            Name = name;
            _nodeCount = _nodeCountDictionary[typeof(TMessage)];
        }

        public LinkedList<List<TMessage>> All
        {
            get { return _all; }
            set
            {
                _all = value;
                BuildOffsets();
            }
        }

        public Dictionary<long, LinkedListNode<List<TMessage>>> Offsets { get; set; } =
            new Dictionary<long, LinkedListNode<List<TMessage>>>();

        [JsonIgnore]
        public IEnumerable<TMessage> Projection => All.SelectMany(l => l);

        public string Name { get; }

        public static void Register(int nodeCount)
        {
            _nodeCountDictionary[typeof(TMessage)] = nodeCount;
        }

        public List<TMessage> Get(long offset)
        {
            return
                Offsets.ContainsKey(offset)
                    ? Offsets[offset].Value
                    : new List<TMessage>();
        }

        public void Append(List<TMessage> list, long offset, bool reverse)
        {
            if (reverse)
                list.Reverse();
            if (offset == 0)
            {
                var numAffected = list.Count / _nodeCount;
                var extraList = list.Count - numAffected * _nodeCount;
                var first = All.First;
                if (first != null && first.Value.Count != 0)
                {
                    var extraInternal = _nodeCount - first.Value.Count;
                    if (extraInternal < list.Count)
                    {
                        first.Value.AddRange(
                            list.GetRange(list.Count - extraInternal, extraInternal));
                    }
                    else
                    {
                        first.Value.AddRange(list);
                    }
                }

                InsertToBeginning(list, numAffected, extraList);
                BuildOffsets();
            }
            else
            {
                Debug.Assert(list.Count <= _nodeCount, $"You passed an illegal list, count = {list.Count}");
                if (Offsets.ContainsKey(offset))
                    Offsets[offset].Value = list;

                if (All.Count < offset / _nodeCount)
                {
                    InsertToEnd(list, offset);
                }
                else
                {
                    if (Offsets.ContainsKey(offset))
                        Offsets[offset].Value = list;
                    else
                    {
                        InsertToEnd(list, offset);
                    }
                }
            }
        }

        private void InsertToEnd(List<TMessage> list, long offset)
        {
            var linkedListNode = new LinkedListNode<List<TMessage>>(list);
            All.AddLast(linkedListNode);
            Offsets.Add(offset, linkedListNode);
        }

        private void InsertToBeginning(List<TMessage> list, int numAffected, int extraList)
        {
            for (var i = numAffected - 1; i >= 0; i--)
            {
                All.AddFirst(
                    new LinkedListNode<List<TMessage>>(
                        list.GetRange(i * _nodeCount, _nodeCount)));
            }
            if (extraList != 0)
            {
                All.AddFirst(
                    new LinkedListNode<List<TMessage>>(
                        list.GetRange(0, extraList)));
            }
        }

        internal void BuildOffsets()
        {
            if (All.Count == 0) return;
            Offsets.Clear();
            var i = 0;
            var start = All.First;
            while (start != null)
            {
                Offsets.Add(i, start);
                start = start.Next;
                i += _nodeCount;
            }
        }

        public static Dialog<TMessage> Empty(string dialogName, long offset)
        {
            var d = new Dialog<TMessage>(dialogName)
            {
                Offsets = new Dictionary<long, LinkedListNode<List<TMessage>>>(),
                All = new LinkedList<List<TMessage>>()
            };
            var linkedListNode = new LinkedListNode<List<TMessage>>(new List<TMessage>());
            d.All.AddFirst(linkedListNode);
            d.Offsets.Add(offset, linkedListNode);
            return d;
        }

        public void Merge(Dialog<TMessage> dialog)
        {
            foreach (var node in dialog.Offsets)
            {
                if (Offsets.ContainsKey(node.Key))
                    Offsets[node.Key].Value = node.Value.Value;
                Append(node.Value.Value, node.Key, false);
            }
        }

        public static Dialog<Message> GetDialog(string dialogName, List<Message> toList, long offset, bool reverse = true)
        {
            var d = new Dialog<Message>(dialogName)
            {
                Offsets = new Dictionary<long, LinkedListNode<List<Message>>>(),
                All = new LinkedList<List<Message>>()
            };
            d.Append(toList, offset, reverse);
            return d;
        }
    }
}