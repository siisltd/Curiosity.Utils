using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Curiosity.Cache.MemoryCache
{
    /// <summary>
    /// Extension methods for <see cref="IMemoryCache"/>.
    /// </summary>
    public static class MemoryCacheExtensions
    {
        /// <summary>
        /// Wrapper for cached value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// All values must be stored in cache only in this wrapper. Because value caching value types is dangerous.
        /// Calling Get method of <see cref="IMemoryCache"/> will return default of value (for value types default is not null).
        /// </remarks>
        private class CachedObject<T>
        {
            /// <summary>
            /// Cached value
            /// </summary>
            #pragma warning disable 8618
            public T Value { get; set; }
            #pragma warning restore 8618
        }
        
        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static T GetOrCreate<T>(
            this IMemoryCache cache,
            object key,
            Func<T> factory,
            TimeSpan absoluteExpiration)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var cachedItem = cache.Get<CachedObject<T>>(key);
            if (cachedItem == null || cachedItem.Value == null) // check Value for more robust
            {
                cachedItem = new CachedObject<T>
                {
                    Value = factory()
                    
                };
                cache.Set(key, cachedItem, absoluteExpiration);
            }

            return cachedItem.Value;
        }
        
        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static T GetOrCreate<T>(
            this IMemoryCache cache,
            object key,
            Func<T> factory,
            MemoryCacheEntryOptions options)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var cachedItem = cache.Get<CachedObject<T>>(key);
            if (cachedItem == null || cachedItem.Value == null) // check Value for more robust
            {
                cachedItem = new CachedObject<T>
                {
                    Value = factory()
                };
                cache.Set(key, cachedItem, options);
            }

            return cachedItem.Value;
        }
        
        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static async Task<T> GetOrCreateAsync<T>(
            this IMemoryCache cache,
            object key,
            Func<Task<T>> factory,
            TimeSpan absoluteExpiration)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            
            var cachedItem = cache.Get<CachedObject<T>>(key);
            if (cachedItem == null || cachedItem.Value == null) // check Value for more robust
            {
                cachedItem = new CachedObject<T>
                {
                    Value = await factory.Invoke()
                };
                cache.Set(key, cachedItem, absoluteExpiration);
            }

            return cachedItem.Value;
        }


        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static async Task<T> GetOrCreateAsync<T>(
            this IMemoryCache cache,
            object key,
            Func<Task<T>> factory,
            MemoryCacheEntryOptions options)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var cachedItem = cache.Get<CachedObject<T>>(key);
            if (cachedItem == null || cachedItem.Value == null) // check Value for more robust
            {
                cachedItem = new CachedObject<T>
                {
                    Value = await factory.Invoke()
                };
                cache.Set(key, cachedItem, options);
            }

            return cachedItem.Value;
        }
        
        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static async ValueTask<T> GetOrCreateValueAsync<T>(
            this IMemoryCache cache,
            object key,
            Func<ValueTask<T>> factory,
            TimeSpan absoluteExpiration)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            
            var cachedItem = cache.Get<CachedObject<T>>(key);
            if (cachedItem == null || cachedItem.Value == null) // check Value for more robust
            {
                cachedItem = new CachedObject<T>
                {
                    Value = await factory.Invoke()
                };
                cache.Set(key, cachedItem, absoluteExpiration);
            }

            return cachedItem.Value;
        }


        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static async ValueTask<T> GetOrCreateValueAsync<T>(
            this IMemoryCache cache,
            object key,
            Func<ValueTask<T>> factory,
            MemoryCacheEntryOptions options)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var cachedItem = cache.Get<CachedObject<T>>(key);
            if (cachedItem == null || cachedItem.Value == null) // check Value for more robust
            {
                cachedItem = new CachedObject<T>
                {
                    Value = await factory.Invoke()
                };
                cache.Set(key, cachedItem, options);
            }

            return cachedItem.Value;
        }
        
        /// <summary>
        /// Try get a value by the key from the cache if null get value from the function and set to the cache.
        /// </summary>
        public static async ValueTask<TItem> GetOrCreateValueAsync<TItem>(
            this IMemoryCache cache, 
            object key, 
            Func<ICacheEntry, ValueTask<TItem>> factory)
        {
            if (cache.TryGetValue(key, out TItem result)) return result;
            
            var entry = cache.CreateEntry(key);
            result = await factory(entry);
            entry.SetValue(result);
                
            // need to manually call dispose instead of having a using
            // in case the factory passed in throws, in which case we
            // do not want to add the entry to the cache
            entry.Dispose();

            return result;
        }
    }
}