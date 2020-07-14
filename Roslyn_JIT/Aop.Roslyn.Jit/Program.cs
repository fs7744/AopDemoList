using System;

namespace Aop.Roslyn.Jit
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestJit.TestJIT();

            //TestProxyGenerator.Test();

            TestProxyJit.Test();

            //new RealClassProxy().Add(5, 10);

            Console.ReadKey();
        }
    }
}
