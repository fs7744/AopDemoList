using System;

namespace Aop.Roslyn.Jit
{
    public static class TestJit
    {
        public static void TestJIT()
        {
            var code = @"
    public class HiJ
    {
        public void Test(string v)
        {
            System.Console.WriteLine($""Hi, {v}!"");
        }
    }";
            var jit = new Jit();
            var syntaxTree = jit.ParseToSyntaxTree(code);
            var compilation = jit.BuildCompilation(syntaxTree);
            var assembly = jit.ComplieToAssembly(compilation);

            // test
            foreach (var item in assembly.GetTypes())
            {
                Console.WriteLine(item.FullName);
                item.GetMethod("Test").Invoke(Activator.CreateInstance(item), new object[] { "joker" });
            }
        }
    }
}