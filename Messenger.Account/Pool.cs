using System;
using System.Collections.Generic;
using Vk.Interface;
using Vk.Types;

namespace Vk
{
    public class Pool<TKey, TValue> where TValue : class
    {
        private Dictionary<TKey, TValue> Mappings { get; }
            = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            ThrowIfNull(key);
            ThrowIfNull(value);

            Mappings[key] = value;
        }

        private static void ThrowIfNull<T>(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
        }

        public void Remove(TKey key)
        {
            Mappings.Remove(key);
        }

        public Result<TValue> Get(TKey key) => Get(key, null);

        public Result<TValue> Get(TKey key, Func<TValue> objectFactory)
        {
            TValue value;
            TryGet(out value, objectFactory, key);
            return new Result<TValue>(value);
        }

        public TValue Get(Func<TValue> objectFactory, TKey key)
        {
            TValue value;
            TryGet(out value, objectFactory, key);
            return new Result<TValue>(value);
        }

        public bool TryGet(out TValue value, Func<TValue> objectFactory, TKey key)
        {
            if (Mappings.ContainsKey(key))
            {
                value = Mappings[key];
                return true;
            }

            var _value = objectFactory();

            if (_value == null)
                throw new ArgumentException($"{nameof(objectFactory)} must not return null", nameof(objectFactory));


            Mappings[key] = _value;

            value = _value;
            return true;
        }
    }

    public class AccountPool<TKey> : Pool<TKey, IAccount>
    {
    }
}