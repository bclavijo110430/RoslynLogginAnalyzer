using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace LoggingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConsoleWriteLineAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(Diagnostics.ConsoleWriteLineRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeConsoleWriteLine, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeConsoleWriteLine(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                return;

            // Check if it's Console.WriteLine
            if (memberAccess.Name.Identifier.Text != "WriteLine" && 
                memberAccess.Name.Identifier.Text != "Write")
                return;

            if (memberAccess.Expression is not IdentifierNameSyntax identifier)
                return;

            if (identifier.Identifier.Text != "Console")
                return;

            // Get the semantic model to check the type
            var semanticModel = context.SemanticModel;
            var symbolInfo = semanticModel.GetSymbolInfo(identifier);
            
            if (symbolInfo.Symbol is not INamedTypeSymbol typeSymbol)
                return;

            if (typeSymbol.ContainingNamespace?.ToDisplayString() != "System")
                return;

            if (typeSymbol.Name != "Console")
                return;

            // Report the diagnostic
            var diagnostic = Diagnostic.Create(
                Diagnostics.ConsoleWriteLineRule,
                invocation.GetLocation(),
                memberAccess.Name.Identifier.Text);

            context.ReportDiagnostic(diagnostic);
        }
    }
} 