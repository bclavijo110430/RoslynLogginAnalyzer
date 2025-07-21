using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace LoggingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StructuredLoggingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(Diagnostics.MissingStructuredParamsRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeStructuredLogging, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeStructuredLogging(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                return;

            // Check if it's a logging method
            var methodName = memberAccess.Name.Identifier.Text;
            if (!IsLoggingMethod(methodName))
                return;

            // Check if there's string concatenation in the arguments
            foreach (var argument in invocation.ArgumentList?.Arguments ?? Enumerable.Empty<ArgumentSyntax>())
            {
                if (ContainsStringConcatenation(argument.Expression))
                {
                    var diagnostic = Diagnostic.Create(
                        Diagnostics.MissingStructuredParamsRule,
                        argument.GetLocation());
                    
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool IsLoggingMethod(string methodName)
        {
            return methodName.StartsWith("Log") || 
                   methodName == "Error" || 
                   methodName == "Warning" || 
                   methodName == "Info" || 
                   methodName == "Debug" || 
                   methodName == "Trace";
        }

        private static bool ContainsStringConcatenation(ExpressionSyntax expression)
        {
            // Check for binary expressions with + operator
            if (expression is BinaryExpressionSyntax binary && 
                binary.IsKind(SyntaxKind.AddExpression))
            {
                return true;
            }

            // Check for interpolated strings (these are generally fine)
            if (expression is InterpolatedStringExpressionSyntax)
            {
                return false;
            }

            // Check for string literals (these are fine)
            if (expression is LiteralExpressionSyntax literal && 
                literal.Kind() == SyntaxKind.StringLiteralExpression)
            {
                return false;
            }

            // Check for method calls that might be string concatenation
            if (expression is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var methodName = memberAccess.Name.Identifier.Text;
                    if (methodName == "Concat" || methodName == "Join" || methodName == "Format")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
} 