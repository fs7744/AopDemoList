using System;

namespace MultipleCache
{
    public interface ICacheAdapter
    {
        // Cache 实现的名字，以此能指定使用哪种缓存实现
        string Name { get; }

        // 尝试获取缓存
        bool TryGetValue<T>(string key, out T result);

        // 存入缓存数据，这里为了简单，我们就只支持ttl过期策略
        void Set<T>(string key, T result, TimeSpan ttl);
    }
}
