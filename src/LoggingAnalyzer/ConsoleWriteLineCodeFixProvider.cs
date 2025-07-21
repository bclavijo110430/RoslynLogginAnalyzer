using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LoggingAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConsoleWriteLineCodeFixProvider)), Shared]
    public class ConsoleWriteLineCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Diagnostics.ConsoleWriteLineRule.Id); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var invocation = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();
            if (invocation == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with structured logging",
                    createChangedDocument: c => ReplaceWithStructuredLoggingAsync(context.Document, invocation, c),
                    equivalenceKey: "ReplaceWithStructuredLogging"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithStructuredLoggingAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            if (root == null)
                return document;

            // Get the arguments from Console.WriteLine
            var arguments = invocation.ArgumentList?.Arguments ?? SyntaxFactory.SeparatedList<ArgumentSyntax>();
            
            // Create a new logging call
            var newInvocation = CreateStructuredLoggingCall(arguments);
            
            var newRoot = root.ReplaceNode(invocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }

        private static InvocationExpressionSyntax CreateStructuredLoggingCall(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            // Create a simple ILogger.LogInformation call
            var loggerIdentifier = SyntaxFactory.IdentifierName("_logger");
            var logMethod = SyntaxFactory.IdentifierName("LogInformation");
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                loggerIdentifier,
                logMethod);

            // Use the same arguments
            var argumentList = SyntaxFactory.ArgumentList(arguments);
            
            return SyntaxFactory.InvocationExpression(memberAccess, argumentList);
        }
    }
} 