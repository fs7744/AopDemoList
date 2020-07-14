using System;
using System.Linq;

namespace Aop.Roslyn.Jit
{
    public static class TestProxyJit
    {
        public static RealClass GenerateRealClassProxy()
        {
            var generator = new ProxyGenerator();
            var code = generator.GenerateProxyCode(typeof(RealClass), (sb, method) => { }, (sb, method) => { sb.Append("r++;"); });
            var jit = new Jit();
            var syntaxTree = jit.ParseToSyntaxTree(code);
            var compilation = jit.BuildCompilation(syntaxTree);
            var assembly = jit.ComplieToAssembly(compilation);
            return Activator.CreateInstance(assembly.GetTypes().First()) as RealClass;
        }

        public static void Test()
        {
            RealClass proxy = GenerateRealClassProxy();
            var i = 5;
            var j = 10;
            Console.WriteLine($"{i} + {j} = {(i + j)}, but proxy is {proxy.Add(i, j)}");
        }
    }
}
