using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace VkData
{
    public class Dialog<TMessage>
    {
        private static Dictionary<Type, int> _nodeCountDictionary = new Dictionary<Type, int>();

        public static void Register(int nodeCount)
        {
            _nodeCountDictionary[typeof(TMessage)] = nodeCount;
        }
        private LinkedList<List<TMessage>> _all = new LinkedList<List<TMessage>>();
        private int _nodeCount;
        private LinkedList<List<TMessage>> All
        {
            get { return _all; }
            set
            {
                _all = value;
                BuildOffsets();
            }
        }

        public Dictionary<long, LinkedListNode<List<TMessage>>> Offsets { get; set; } = new Dictionary<long, LinkedListNode<List<TMessage>>>();

        public List<TMessage> Get(long offset)
        {
            return
                Offsets.ContainsKey(offset)
                ? Offsets[offset].Value
                : new List<TMessage>();
        }

        public void Append(List<TMessage> list, long offset, bool reverse)
        {
            if(reverse)
               list.Reverse();
            if (offset == 0)
            {
                var firstCount = All.First.Value.Count;
                var numAffected = list.Count / _nodeCount;
                var extraList = list.Count - numAffected * _nodeCount;
                if (firstCount != 0)
                {
                    var extraInternal = _nodeCount - firstCount;
                    All.First.Value.AddRange(
                        list.GetRange(list.Count - extraInternal, extraInternal));
                }

                InsertToBeginning(list, numAffected, extraList);
                BuildOffsets();
            }
            else
            {
                Debug.Assert(list.Count <= _nodeCount, $"You passed an illegal list, count = {list.Count}");
                Offsets[offset].Value = list;
            }
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
            while (start.Next != null)
            {
                Offsets.Add(i++, start);
                start = start.Next;
            }
        }

        public Dialog<TMessage> Empty(long offset)
        {
           var d = new Dialog<TMessage>
           {
              Offsets =  new Dictionary<long, LinkedListNode<List<TMessage>>>(),
              All = new LinkedList<List<TMessage>>()
           };
           d.All.First.Value = new List<TMessage>();
            return d;
        }

        [JsonIgnore]
        public IEnumerable<TMessage> Projection => All.SelectMany(l => l);

        public Dialog()
        {
            _nodeCount = _nodeCountDictionary[typeof(TMessage)];
        }

        public void Merge(Dialog<TMessage> dialog)
        { 
           foreach(var node in dialog.Offsets)
           {
               if(Offsets.ContainsKey(node.Key))
                 throw new ArgumentException($"Offset {node.Key} is alredy present in dialog");
               Append(node.Value.Value, node.Key, false);
           }
        }
    }
}