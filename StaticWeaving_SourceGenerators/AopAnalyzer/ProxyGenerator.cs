using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AopAnalyzer
{
    [Generator]
    public class ProxyGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            // retreive the populated receiver
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;
            try
            {
                // 简单测试aop 生成
                Action<StringBuilder, IMethodSymbol> beforeCall = (sb, method) => { };
                Action<StringBuilder, IMethodSymbol> afterCall = (sb, method) => { sb.Append("r++;"); };

                // 获取生成结果
                var code = receiver.SyntaxNodes
                 .Select(i => context.Compilation.GetSemanticModel(i.SyntaxTree).GetDeclaredSymbol(i) as INamedTypeSymbol)
                 .Where(i => i != null && !i.IsStatic)
                 .Select(i => ProxyCodeGenerator.GenerateProxyCode(i, beforeCall, afterCall))
                 .First();

                context.AddSource("code.cs", SourceText.From(code, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                // 失败汇报
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("n001", ex.ToString(), ex.ToString(), "AOP.Generate", DiagnosticSeverity.Warning, true), Location.Create("code.cs", TextSpan.FromBounds(0, 0), new LinePositionSpan())));
            }
        }

        public void Initialize(InitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <summary>
        /// 语法树定义收集器，可以在这里过滤生成器所需
        /// </summary>
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            internal List<SyntaxNode> SyntaxNodes { get; } = new List<SyntaxNode>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax)
                {
                    SyntaxNodes.Add(syntaxNode);
                }
            }
        }
    }
}