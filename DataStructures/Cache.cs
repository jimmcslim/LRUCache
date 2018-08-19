using System;
using System.Collections.Generic;

namespace FoneDynamics.DataStructures
{
    /// <summary>
    /// An implementation of <see cref="ICache{TKey,TValue}"/> that implements a
    /// 'Least Recently Used' eviction policy. The cache must be provided with
    /// a maximum number of items when constructed. If adding an item will cause
    /// this limit to be exceeded, then the least recently used item will be removed
    /// from the cache as part of the add operations (i.e. there are no background
    /// threads that will periodically remove the least recently used items). 
    /// </summary>
    public class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly int _maximumSize;
        private readonly object _lock = new object();

        private readonly IDictionary<TKey, CacheItem<TKey, TValue>> _cacheItems;
        private readonly LinkedList<TKey> _mostRecentlyUsedKeys;

        public Cache(int maximumSize)
        {
            if (maximumSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumSize), maximumSize, "maximumSize must be 1 or greater.");
            }
            _maximumSize = maximumSize;
            _cacheItems = new Dictionary<TKey, CacheItem<TKey, TValue>>(maximumSize);
            _mostRecentlyUsedKeys = new LinkedList<TKey>();
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "key cannot be null.");
            }

            lock (_lock)
            {
                if (!_cacheItems.TryGetValue(key, out var cacheItem)) // Adding item
                {
                    EvictLeastRecentlyUsedIfNecessary();
                    var newItemNode = _mostRecentlyUsedKeys.AddFirst(key);
                    _cacheItems[key] = new CacheItem<TKey, TValue>(newItemNode, value);
                }
                else // Updating item
                {
                    UpdateMostRecentlyUsedKeys(key, cacheItem);
                    cacheItem.Value = value;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "key cannot be null.");
            }

            lock (_lock)
            {
                if (!_cacheItems.TryGetValue(key, out var cacheItem))
                {
                    value = default(TValue);
                    return false;
                }

                UpdateMostRecentlyUsedKeys(key, cacheItem);

                value = cacheItem.Value;
                return true;
            }
        }

        private void EvictLeastRecentlyUsedIfNecessary()
        {
            if (_cacheItems.Count < _maximumSize)
            {
                return;
            }

            var leastRecentlyUsedItem = _mostRecentlyUsedKeys.Last;
            _cacheItems.Remove(leastRecentlyUsedItem.Value);
            _mostRecentlyUsedKeys.Remove(leastRecentlyUsedItem);
        }

        private void UpdateMostRecentlyUsedKeys(TKey key, CacheItem<TKey, TValue> cacheItem)
        {
            _mostRecentlyUsedKeys.Remove(cacheItem.Node);
            var node = _mostRecentlyUsedKeys.AddFirst(key);
            cacheItem.Node = node;
        }
    }
}