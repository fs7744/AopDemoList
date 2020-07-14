using System;

namespace Aop.Roslyn.Jit
{
    public static class TestProxyGenerator
    {
        public static void Test()
        {
            var generator = new ProxyGenerator();
            var code = generator.GenerateProxyCode(typeof(RealClass), (sb, method) => { }, (sb, method) => { sb.Append("r++;"); });
            Console.WriteLine(code);
        }
    }
}
