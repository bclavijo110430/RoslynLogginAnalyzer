using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;

namespace LoggingAnalyzer.Tests
{
    public class StructuredLoggingAnalyzerTests
    {
        private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<LoggingAnalyzer.StructuredLoggingAnalyzer, DefaultVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }

        [Fact]
        public async Task LogWithStringConcatenation_ShouldReportDiagnostic()
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
        var userId = 123;
        _logger.LogInformation(""User "" + userId + "" logged in"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.MissingStructuredParamsRule)
                .WithLocation(14, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogWithStringFormat_ShouldReportDiagnostic()
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
        var userId = 123;
        _logger.LogInformation(string.Format(""User {0} logged in"", userId));
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.MissingStructuredParamsRule)
                .WithLocation(14, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogWithStringConcat_ShouldReportDiagnostic()
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
        var userId = 123;
        _logger.LogInformation(string.Concat(""User "", userId.ToString(), "" logged in""));
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.MissingStructuredParamsRule)
                .WithLocation(14, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogWithInterpolatedString_ShouldNotReportDiagnostic()
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
        var userId = 123;
        _logger.LogInformation($""User {userId} logged in"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogWithStructuredParameters_ShouldNotReportDiagnostic()
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
        var userId = 123;
        _logger.LogInformation(""User {UserId} logged in"", userId);
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogWithSimpleString_ShouldNotReportDiagnostic()
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
        _logger.LogInformation(""User logged in successfully"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogWithStringJoin_ShouldReportDiagnostic()
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
        var parts = new[] { ""User"", ""123"", ""logged"", ""in"" };
        _logger.LogInformation(string.Join("" "", parts));
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.MissingStructuredParamsRule)
                .WithLocation(14, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task NonLoggingMethodWithConcatenation_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    public void DoSomething()
    {
        var userId = 123;
        var message = ""User "" + userId + "" logged in"";
        Console.WriteLine(message);
    }
}";

            await VerifyAnalyzerAsync(test);
        }
    }
} 