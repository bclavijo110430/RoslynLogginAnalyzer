using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoggingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SensitiveInfoAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(Diagnostics.SensitiveInfoRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSensitiveInfo, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeSensitiveInfo(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                return;

            // Check if it's a logging method
            var methodName = memberAccess.Name.Identifier.Text;
            if (!IsLoggingMethod(methodName))
                return;

            // Check all string arguments for sensitive information
            foreach (var argument in invocation.ArgumentList?.Arguments ?? Enumerable.Empty<ArgumentSyntax>())
            {
                if (argument.Expression is LiteralExpressionSyntax literal && 
                    literal.Kind() == SyntaxKind.StringLiteralExpression)
                {
                    var stringValue = literal.Token.ValueText;
                    var sensitivePattern = DetectSensitivePattern(stringValue);
                    
                    if (!string.IsNullOrEmpty(sensitivePattern))
                    {
                        var diagnostic = Diagnostic.Create(
                            Diagnostics.SensitiveInfoRule,
                            argument.GetLocation(),
                            sensitivePattern);
                        
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (argument.Expression is InterpolatedStringExpressionSyntax interpolated)
                {
                    // Check interpolated strings
                    var interpolatedText = interpolated.ToString();
                    var sensitivePattern = DetectSensitivePattern(interpolatedText);
                    
                    if (!string.IsNullOrEmpty(sensitivePattern))
                    {
                        var diagnostic = Diagnostic.Create(
                            Diagnostics.SensitiveInfoRule,
                            argument.GetLocation(),
                            sensitivePattern);
                        
                        context.ReportDiagnostic(diagnostic);
                    }
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
                   methodName == "Trace" ||
                   methodName == "WriteLine" ||
                   methodName == "Write";
        }

        private static string? DetectSensitivePattern(string text)
        {
            var lowerText = text.ToLowerInvariant();
            
            // Check for sensitive keywords
            foreach (var keyword in SensitivePatterns.SensitiveKeywords)
            {
                if (lowerText.Contains(keyword))
                {
                    return $"Contains sensitive keyword: '{keyword}'";
                }
            }

            // Check for regex patterns
            foreach (var pattern in SensitivePatterns.SensitivePatternsRegex)
            {
                try
                {
                    if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                    {
                        return $"Matches sensitive pattern: {pattern}";
                    }
                }
                catch
                {
                    // Invalid regex pattern, skip
                }
            }

            return null;
        }
    }
} 