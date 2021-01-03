using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MultipleCache
{
    public class DoCacheTest
    {
        [Cache(CacheKey = nameof(Do), CacheName = "memory", Order = 0, Ttl = "00:00:01")]   // 1秒过期
        [Cache(CacheKey = nameof(Do), CacheName = "distribute", Order = 1, Ttl = "00:00:05")]  // 5秒过期
        public virtual Task<string> Do() => Task.FromResult(DateTime.Now.ToString());
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var sut = new ServiceCollection()
                   .AddTransient<DoCacheTest>()
                   .ConfigureAop(i => i.GlobalInterceptors.Add(new CacheInterceptor()))  // 设置Cache拦截器
                   .AddMemoryCache()
                   .AddDistributedMemoryCache() // 为了测试，我们就不使用redis之类的东西了，用个内存实现模拟就好
                   .AddSingleton<ICacheAdapter, MemoryCacheAdapter>()  // 添加缓存适配器
                   .AddSingleton<ICacheAdapter>(i => new DistributedCacheAdapter(i.GetRequiredService<IDistributedCache>(), "distribute"))
                   .BuildServiceProvider()
                  .GetRequiredService<DoCacheTest>();

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"Get: {await sut.Do()}");
                await Task.Delay(500);  // 每隔半秒，观察缓存变化
            }
        }
    }
}
