using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace MakeMemoryChacheSimple
{
    static class Program
    {
        static void Main(string[] args)
        {
            var client = new ServiceCollection()
                .AddMemoryCache()
                .AddSingleton<ITestCacheClient, TestCacheClient>()
                .ConfigureAop()
                .BuildServiceProvider()
                .GetRequiredService<ITestCacheClient>();
            Console.WriteLine(client.Say("Hello World!"));
            Console.WriteLine(client.Say("Hello Two!"));
            Thread.Sleep(3000);
            Console.WriteLine(client.Say("Hello Two!"));
        }
    }
}
