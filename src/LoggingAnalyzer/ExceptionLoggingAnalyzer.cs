using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace LoggingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionLoggingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(Diagnostics.ExceptionLoggingRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeExceptionLogging, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeExceptionLogging(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                return;

            // Check if it's a logging method
            var methodName = memberAccess.Name.Identifier.Text;
            if (!IsLoggingMethod(methodName))
                return;

            // Check if there's an exception parameter
            var exceptionArgument = invocation.ArgumentList?.Arguments
                .FirstOrDefault(arg => IsExceptionType(context.SemanticModel, arg.Expression));

            if (exceptionArgument == null)
                return;

            // Check if the exception is being logged properly
            if (IsExceptionLoggedProperly(exceptionArgument.Expression))
                return;

            // Report the diagnostic
            var diagnostic = Diagnostic.Create(
                Diagnostics.ExceptionLoggingRule,
                exceptionArgument.GetLocation());

            context.ReportDiagnostic(diagnostic);
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

        private static bool IsExceptionType(SemanticModel semanticModel, ExpressionSyntax expression)
        {
            var typeInfo = semanticModel.GetTypeInfo(expression);
            if (typeInfo.Type == null)
                return false;

            var type = typeInfo.Type;
            while (type != null)
            {
                if (type.Name == "Exception" && type.ContainingNamespace?.ToDisplayString() == "System")
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        private static bool IsExceptionLoggedProperly(ExpressionSyntax exceptionExpression)
        {
            // Check if the exception is being used with .ToString() or .Message
            if (exceptionExpression is MemberAccessExpressionSyntax memberAccess)
            {
                var memberName = memberAccess.Name.Identifier.Text;
                return memberName == "ToString" || memberName == "Message";
            }

            // Check if it's a method call on the exception
            if (exceptionExpression is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax invocationMember)
                {
                    var memberName = invocationMember.Name.Identifier.Text;
                    return memberName == "ToString";
                }
            }

            return false;
        }
    }
} 