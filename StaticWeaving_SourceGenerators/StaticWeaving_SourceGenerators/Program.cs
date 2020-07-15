using System;

namespace StaticWeaving_SourceGenerators
{
    static class Program
    {
        static void Main(string[] args)
        {
            var proxy = new RealClassProxy(); // 对，生成的新代码可以ide里面直接用，就是这么强大，只要编译一次就看的到了
            var i = 5;
            var j = 10;
            Console.WriteLine($"{i} + {j} = {(i + j)}, but proxy is {proxy.Add(i, j)}");
            Console.ReadKey();
        }
    }
}
