using System.Collections.Generic;
using System.Linq;
using Cronos;
using Dikubot.DataLayer.Cronjob;
using Microsoft.Graph;

namespace Dikubot.DataLayer.Caching
{
    /// <summary>
    /// This cache is unreliable in the sense that items are not cached individually, meaning items are cached between x and y minutes, instead of a constant amount of time.
    /// The reason the cache is implemented like this is really just because I thought it was a fun way of implementing cache
    /// </summary>
    public class Cache<TKey, TValue>
    {
        private int _swap;
        private CronTask _cronTask;
        private Scheduler _scheduler = new Scheduler();

        
        private Dictionary<TKey, TValue> activeStorage = new Dictionary<TKey, TValue>();
        private Dictionary<TKey, TValue> inactiveStorage = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Initialize a cache. Cached items will be cached for at least swap minutes, and at most 2 times swap minutes
        /// </summary>
        /// <param name="swap">Cached items will be cached for at least swap minutes, and at most 2 times swap minutes</param>
        public Cache(int minuteSwap)
        {
            _swap = minuteSwap;
            _cronTask = new CronTask(CronExpression.Parse($"*/{minuteSwap} * * * *"), Swap);

        }
        
        /// <summary>
        /// Swap simply switches the inactive and active storage
        /// The active storage is the storage items is added to, until the next swap, while the inactive storage gets deleted at the next swap
        /// </summary>
        private void Swap()
        {
            inactiveStorage = new Dictionary<TKey, TValue>(activeStorage);
            activeStorage.Clear();
        }

        /// <summary>
        /// Get the value in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual TValue GetValue(TKey key)
        {
            TValue value;
            if (activeStorage.TryGetValue(key, out value))
            {
                return value;
            }
            
            return inactiveStorage.TryGetValue(key, out value) ? value : default;
        }

        /// <summary>
        /// Set a key's value in the active cache storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected virtual void SetValue(TKey key, TValue value)
        {
            activeStorage[key] = value;
        }
        

        /// <summary>
        /// Add a key and value to the active cache storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            SetValue(key, value);
        }

        /// <summary>
        /// Checks whether the active or inactive storage contains the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Whether the key exists in the active or inactive storage</returns>
        public bool ContainsKey(TKey key)
        {
            return activeStorage.ContainsKey(key) || inactiveStorage.ContainsKey(key);
        }

        /// <summary>
        /// Remove a key from the cache
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            if (activeStorage.ContainsKey(key))
            {
                activeStorage.Remove(key);
                return;
            }

            if (inactiveStorage.ContainsKey(key))
            {
                inactiveStorage.Remove(key);
            }
        }
        
        public TValue this[TKey key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }
        
        
        
        
    }
}