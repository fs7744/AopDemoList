using System;

namespace EmitAopDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RealClass proxy = ProxyGenerator.Generate<RealClass>(typeof(AddOneInterceptor));
            var i = 5;
            var j = 10;
            Console.WriteLine($"{i} + {j} = {(i + j)}, but proxy is {proxy.Add(i, j)}");
            Console.ReadLine();
        }
    }
}