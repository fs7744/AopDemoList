using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Norns.Urd;
using System;
using System.Threading.Tasks;

namespace MakeMemoryChacheSimple
{
    public class CacheAttribute : AbstractInterceptorAttribute
    {
        private readonly TimeSpan absoluteExpirationRelativeToNow;
        private readonly string cacheKey;

        // 为了简单，缓存策略我们就先只支持TTL 存活固定时间
        public CacheAttribute(string cacheKey, string absoluteExpirationRelativeToNow)
        {
            this.cacheKey = cacheKey;
            this.absoluteExpirationRelativeToNow = TimeSpan.Parse(absoluteExpirationRelativeToNow);
        }

        public override async Task InvokeAsync(AspectContext context, AsyncAspectDelegate next)
        {
            // 整个代码基本和我们直接使用 MemoryCache 一样
            var cache = context.ServiceProvider.GetRequiredService<IMemoryCache>();
            var r = await cache.GetOrCreateAsync(cacheKey, async e =>
            {
                await next(context); // 所以真正实现的方法逻辑都在 next 中，所以调用它就好了
                e.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
                return context.ReturnValue;  // 结果都在ReturnValue ， 这里为了简单，就不写 void / Task<T> / ValueTask<T> 等等 各种返回值的兼容代码了
            });
            context.ReturnValue = r; // 设置 ReturnValue， 由于缓存有效期内， next不会被调用， 所以ReturnValue不会有值，我们需要将缓存结果设置到 ReturnValue
        }
    }
}