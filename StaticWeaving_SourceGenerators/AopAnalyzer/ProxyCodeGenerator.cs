using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;
//using Newtonsoft;

namespace AopAnalyzer
{
    public static class ProxyCodeGenerator
    {
        public static string GenerateProxyCode(INamedTypeSymbol type, Action<StringBuilder, IMethodSymbol> beforeCall, Action<StringBuilder, IMethodSymbol> afterCall)
        {
            //string v = Newtonsoft.Json.JsonConvert.ToString(DateTime.Now);
            var sb = new StringBuilder();
            sb.Append($"namespace {type.ContainingNamespace.ToDisplayString()} {{");
            sb.Append($"{(type.DeclaredAccessibility == Accessibility.Public ? "public": "")} class {type.Name}Proxy : {type.Name} {{ ");
            foreach (var method in type.GetMembers().Select(i => i as IMethodSymbol).Where(i => i != null && i.MethodKind != MethodKind.Constructor))
            {
                GenerateProxyMethod(beforeCall, afterCall, sb, method);
            }
            sb.Append(" } }");
            return sb.ToString();
        }

        private static void GenerateProxyMethod(Action<StringBuilder, IMethodSymbol> beforeCall, Action<StringBuilder, IMethodSymbol> afterCall, StringBuilder sb, IMethodSymbol method)
        {
            var ps = method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}");

            sb.Append($"{(method.DeclaredAccessibility == Accessibility.Public ? "public" : "")} override {method.ReturnType.ToDisplayString()} {method.Name}({string.Join(",", ps)}) {{");

            sb.Append($"{method.ReturnType.ToDisplayString()} r = default;");

            beforeCall(sb, method);

            sb.Append($"r = base.{method.Name}({string.Join(",", method.Parameters.Select(p => p.Name))});");

            afterCall(sb, method);

            sb.Append("return r; }");
        }
    }
}
