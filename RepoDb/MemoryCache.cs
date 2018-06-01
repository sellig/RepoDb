﻿using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Linq;
using System.Collections;

namespace RepoDb
{
    /// <summary>
    /// An object used for caching a result in the repository <i>Query</i> operation. This object is the default
    /// memory cache object used by the <i>RepoDb</i> repositories.
    /// </summary>
    public class MemoryCache : ICache
    {
        private static object _syncLock = new object();
        private readonly IList<ICacheItem> _cacheList;

        internal MemoryCache()
        {
            _cacheList = new List<ICacheItem>();
        }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        public void Add(string key, object value)
        {
            lock (_syncLock)
            {
                var item = GetItem(key);
                if (item == null)
                {
                    _cacheList.Add(new CacheItem(key, value));
                }
                else
                {
                    var cacheItem = (CacheItem)item;
                    cacheItem.Value = value;
                    cacheItem.Timestamp = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="item">The cache item to be added in the collection. This object must implement the <i>RepoDb.Interfaces.ICacheItem<paramref name="item"/> interface.</param>
        public void Add(ICacheItem item)
        {
            lock (_syncLock)
            {
                var cache = GetItem(item.Key);
                if (cache == null)
                {
                    _cacheList.Add(item);
                }
                else
                {
                    cache.Value = item.Value;
                    cache.Timestamp = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Clears the collection of the cache.
        /// </summary>
        public void Clear()
        {
            _cacheList.Clear();
        }

        /// <summary>
        /// Checks whether the key is present in the collection.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        public bool Contains(string key)
        {
            return GetItem(key) != null;
        }

        /// <summary>
        /// Gets an object from the cache collection.
        /// </summary>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>An object from the cache collection based on the given key.</returns>
        public object Get(string key)
        {
            var cacheItem = GetItem(key);
            return cacheItem?.Value;
        }

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICacheItem> GetEnumerator()
        {
            return _cacheList.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cacheList.GetEnumerator();
        }

        private ICacheItem GetItem(string key)
        {
            return _cacheList.FirstOrDefault(cacheItem =>
                string.Equals(cacheItem.Key, key, StringComparison.CurrentCulture));
        }

        /// <summary>
        /// Removes the item from the cache collection.
        /// </summary>
        /// <param name="key">
        /// The key of the item to be removed from the cache collection. If the given key is not present, this method will ignore it.
        /// </param>
        public void Remove(string key)
        {
            var item = GetItem(key);
            if (item != null)
            {
                _cacheList.Remove(item);
            }
        }
    }
}
