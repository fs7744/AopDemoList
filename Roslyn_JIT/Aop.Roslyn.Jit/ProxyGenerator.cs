using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aop.Roslyn.Jit
{
    public class ProxyGenerator
    {
        public string GenerateProxyCode(Type type, Action<StringBuilder, MethodBase> beforeCall, Action<StringBuilder, MethodBase> afterCall)
        {
            var sb = new StringBuilder();
            sb.Append($"{(type.IsPublic ? "public" : "")} class {type.Name}Proxy : {type.FullName} {{ ");
            foreach (var method in type.GetTypeInfo().DeclaredMethods)
            {
                GenerateProxyMethod(beforeCall, afterCall, sb, method);
            }
            sb.Append(" }");
            return sb.ToString();
        }

        private static void GenerateProxyMethod(Action<StringBuilder, MethodBase> beforeCall, Action<StringBuilder, MethodBase> afterCall, StringBuilder sb, MethodInfo method)
        {
            var ps = method.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}");

            sb.Append($"{(method.IsPublic ? "public" : "")} override {method.ReturnType.FullName} {method.Name}({string.Join(",", ps)}) {{");

            sb.Append($"{method.ReturnType.FullName} r = default;");

            beforeCall(sb, method);

            sb.Append($"r = base.{method.Name}({string.Join(",", method.GetParameters().Select(p => p.Name))});");

            afterCall(sb, method);

            sb.Append("return r; }");
        }
    }
}
