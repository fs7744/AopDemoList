using System;

namespace MultipleCache
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CacheAttribute : Attribute
    {
        // 由于多个缓存实现，我们需要有使用顺序指定
        public int Order { get; set; }
        public string CacheKey { get; set; }
        public string Ttl { get; set; }
        public string CacheName { get; set; }
    }
}