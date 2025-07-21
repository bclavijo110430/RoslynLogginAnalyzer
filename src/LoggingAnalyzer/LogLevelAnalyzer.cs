using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace LoggingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LogLevelAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(Diagnostics.IncorrectLogLevelRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeLogLevel, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeLogLevel(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                return;

            var methodName = memberAccess.Name.Identifier.Text;
            var currentLevel = GetLogLevel(methodName);
            
            if (currentLevel == LogLevel.Unknown)
                return;

            // Get the message argument
            var messageArgument = GetMessageArgument(invocation);
            if (messageArgument == null)
                return;

            var message = GetStringValue(messageArgument.Expression);
            if (string.IsNullOrEmpty(message))
                return;

            var suggestedLevel = AnalyzeMessageContent(message);
            
            if (suggestedLevel != LogLevel.Unknown && suggestedLevel != currentLevel)
            {
                var diagnostic = Diagnostic.Create(
                    Diagnostics.IncorrectLogLevelRule,
                    memberAccess.Name.GetLocation(),
                    suggestedLevel.ToString(),
                    currentLevel.ToString());
                
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static LogLevel GetLogLevel(string methodName)
        {
            return methodName.ToLowerInvariant() switch
            {
                "logerror" or "error" => LogLevel.Error,
                "logwarning" or "warning" => LogLevel.Warning,
                "loginformation" or "loginfo" or "info" => LogLevel.Information,
                "logdebug" or "debug" => LogLevel.Debug,
                "logtrace" or "trace" => LogLevel.Trace,
                _ => LogLevel.Unknown
            };
        }

        private static ArgumentSyntax? GetMessageArgument(InvocationExpressionSyntax invocation)
        {
            var arguments = invocation.ArgumentList?.Arguments;
            if (arguments == null || arguments.Value.Count == 0)
                return null;

            // Usually the first argument is the message
            return arguments.Value[0];
        }

        private static string GetStringValue(ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax literal && 
                literal.Kind() == SyntaxKind.StringLiteralExpression)
            {
                return literal.Token.ValueText;
            }

            if (expression is InterpolatedStringExpressionSyntax interpolated)
            {
                return interpolated.ToString();
            }

            return string.Empty;
        }

        private static LogLevel AnalyzeMessageContent(string message)
        {
            var lowerMessage = message.ToLowerInvariant();

            // Error indicators
            if (ContainsErrorIndicators(lowerMessage))
                return LogLevel.Error;

            // Warning indicators
            if (ContainsWarningIndicators(lowerMessage))
                return LogLevel.Warning;

            // Debug indicators
            if (ContainsDebugIndicators(lowerMessage))
                return LogLevel.Debug;

            // Trace indicators
            if (ContainsTraceIndicators(lowerMessage))
                return LogLevel.Trace;

            // Default to Information for general messages
            return LogLevel.Information;
        }

        private static bool ContainsErrorIndicators(string message)
        {
            var errorKeywords = new[] { "error", "exception", "failed", "failure", "crash", "fatal", "critical" };
            return errorKeywords.Any(keyword => message.Contains(keyword));
        }

        private static bool ContainsWarningIndicators(string message)
        {
            var warningKeywords = new[] { "warning", "warn", "deprecated", "obsolete", "retry", "timeout", "slow" };
            return warningKeywords.Any(keyword => message.Contains(keyword));
        }

        private static bool ContainsDebugIndicators(string message)
        {
            var debugKeywords = new[] { "debug", "entering", "exiting", "step", "checkpoint", "state" };
            return debugKeywords.Any(keyword => message.Contains(keyword));
        }

        private static bool ContainsTraceIndicators(string message)
        {
            var traceKeywords = new[] { "trace", "verbose", "detail", "internal", "private" };
            return traceKeywords.Any(keyword => message.Contains(keyword));
        }

        private enum LogLevel
        {
            Unknown,
            Trace,
            Debug,
            Information,
            Warning,
            Error
        }
    }
} 