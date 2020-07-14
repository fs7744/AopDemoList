using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Aop.Roslyn.Jit
{
    public class Jit
    {
        public SyntaxTree ParseToSyntaxTree(string code)
        {
            var parseOptions = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[] { "RELEASE" });
            // 有许多其他配置项，最简单这些就可以了

            return CSharpSyntaxTree.ParseText(code, parseOptions);
        }

        public CSharpCompilation BuildCompilation(SyntaxTree syntaxTree)
        {
            var compilationOptions = new CSharpCompilationOptions(
                                   concurrentBuild: true,
                                   metadataImportOptions: MetadataImportOptions.All,
                                   outputKind: OutputKind.DynamicallyLinkedLibrary,
                                   optimizationLevel: OptimizationLevel.Release,
                                   allowUnsafe: true,
                                   platform: Platform.AnyCpu,
                                   checkOverflow: false,
                                   assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            // 有许多其他配置项，最简单这些就可以了

            var references = AppDomain.CurrentDomain.GetAssemblies()
               .Where(i => !i.IsDynamic && !string.IsNullOrWhiteSpace(i.Location))
               .Distinct()
               .Select(i => MetadataReference.CreateFromFile(i.Location));
            // 获取编译时所需用到的dll， 这里我们直接简单一点 copy 当前执行环境的

            return CSharpCompilation.Create("code.cs", new SyntaxTree[] { syntaxTree }, references, compilationOptions);
        }

        public Assembly ComplieToAssembly(CSharpCompilation compilation)
        {
            using (var stream = new MemoryStream())
            {
                var restult = compilation.Emit(stream);
                if (restult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return AssemblyLoadContext.Default.LoadFromStream(stream);
                }
                else
                {
                    throw new Exception(restult.Diagnostics.Select(i => i.ToString()).DefaultIfEmpty().Aggregate((i, j) => i + j));
                }
            }
        }
    }
}
