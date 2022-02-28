﻿using System;

using Newtonsoft.Json;

using Umbraco.Core.Cache;

namespace Umbraco.Cms.Integrations.Shared.Services
{
    public class CacheHelper : ICacheHelper
    {
        private readonly IAppPolicyCache _cache;

        public CacheHelper(AppCaches appCaches)
        {
            _cache = appCaches.RuntimeCache;
        }

        public bool TryGetCachedItem<T>(string key, out T item) where T : class
        {
            var serializedItem = _cache.GetCacheItem<string>(key);

            item = string.IsNullOrEmpty(serializedItem)
                ? null
                : JsonConvert.DeserializeObject<T>(serializedItem);

            return !string.IsNullOrEmpty(serializedItem);
        }

        public void AddCachedItem(string key, string item)
        {
            _cache.InsertCacheItem(key, () => item, TimeSpan.FromHours(1));
        }

        public void ClearCachedItems()
        {
            _cache.Clear();
        }
    }
}