using Microsoft.Extensions.DependencyInjection;
using Norns.Urd;
using Norns.Urd.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultipleCache
{
    public class CacheInterceptor : AbstractInterceptor
    {
        public override bool CanAspect(MethodReflector method)
        {
            return method.IsDefined<CacheAttribute>();  // 限制只对有缓存定义的方法起效
        }

        public override async Task InvokeAsync(AspectContext context, AsyncAspectDelegate next)
        {
            var caches = context.ServiceProvider.GetRequiredService<IEnumerable<ICacheAdapter>>()
                .ToDictionary(i => i.Name, StringComparer.OrdinalIgnoreCase);

            var cas = context.Method.GetReflector()
                .GetCustomAttributes<CacheAttribute>()
                .OrderBy(i => i.Order)
                .ToArray();

            // 为了简单，我们就使用最简单的反射形式调用
            var m = typeof(CacheInterceptor).GetMethod(nameof(CacheInterceptor.GetOrCreateAsync))
                 .MakeGenericMethod(context.Method.ReturnType.GetGenericArguments()[0]);
            await (Task)m.Invoke(this, new object[] { caches, cas, context, next, 0 });
        }

        public async Task<T> GetOrCreateAsync<T>(Dictionary<string, ICacheAdapter> adapters, CacheAttribute[] options, AspectContext context, AsyncAspectDelegate next, int index)
        {
            if (index >= options.Length)
            {
                Console.WriteLine($"No found Cache at {DateTime.Now}.");
                // 所有cache 都找完了，没有找到有效cache，所以需要拿真正的结果
                await next(context);
                // 为了简单，我们就只支持 Task<T> 的结果
                return ((Task<T>)context.ReturnValue).Result;
            }

            var op = options[index];
            T result;
            var cacheName = op.CacheName;
            if (adapters.TryGetValue(cacheName, out var adapter))
            {
                if (!adapter.TryGetValue<T>(op.CacheKey, out result))
                {
                    // 当前缓存找不到结果，移到下一个缓存获取结果
                    result = await GetOrCreateAsync<T>(adapters, options, context, next, ++index);
                    adapter.Set(op.CacheKey, result, TimeSpan.Parse(op.Ttl)); // 更新当前缓存实现的存储
                    context.ReturnValue = Task.FromResult(result); // 为了简单，我们就在这儿更新返回结果，其实不该在这儿的，为什么，大家可以猜一猜为什么？
                }
                else
                {
                    Console.WriteLine($"Get Cache From {cacheName} at {DateTime.Now}.");
                    context.ReturnValue = Task.FromResult(result); // 为了简单，我们就在这儿更新返回结果，其实不该在这儿的，为什么，大家可以猜一猜为什么？
                }
            }
            else
            {
                throw new ArgumentException($"No such cache: {cacheName}.");
            }

            return result;
        }
    }
}