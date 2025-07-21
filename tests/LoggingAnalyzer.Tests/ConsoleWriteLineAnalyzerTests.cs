using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;

namespace LoggingAnalyzer.Tests
{
    public class ConsoleWriteLineAnalyzerTests
    {
        private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<LoggingAnalyzer.ConsoleWriteLineAnalyzer, DefaultVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }

        [Fact]
        public async Task ConsoleWriteLine_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(""Hello World"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.ConsoleWriteLineRule)
                .WithLocation(8, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task ConsoleWrite_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        Console.Write(""Hello World"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.ConsoleWriteLineRule)
                .WithLocation(8, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task ConsoleWriteLineWithFormat_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(""Hello {0}"", ""World"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.ConsoleWriteLineRule)
                .WithLocation(8, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task NonConsoleWriteLine_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        var console = new Console();
        console.WriteLine(""Hello World"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LoggerWriteLine_ShouldNotReportDiagnostic()
        {
            var test = @"
using Microsoft.Extensions.Logging;

class Program
{
    private readonly ILogger _logger;

    public Program(ILogger logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation(""Hello World"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task ConsoleOtherMethod_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        Console.Clear();
    }
}";

            await VerifyAnalyzerAsync(test);
        }
    }
} 