using System;
using System.Collections.Generic;

namespace FoneDynamics.DataStructures
{
    internal class CacheItem<TKey, TValue>
    {
        public LinkedListNode<TKey> Node { get; set; }
        public TValue Value { get; set; }

        public CacheItem(LinkedListNode<TKey> node, TValue value)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Value = value;
        }
    }
}