using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;

namespace LoggingAnalyzer.Tests
{
    public class SensitiveInfoAnalyzerTests
    {
        private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<LoggingAnalyzer.SensitiveInfoAnalyzer, DefaultVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }

        [Fact]
        public async Task LogPassword_ShouldReportDiagnostic()
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
        _logger.LogInformation(""User password is: mypassword123"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogApiKey_ShouldReportDiagnostic()
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
        _logger.LogInformation(""API Key: sk-1234567890abcdef"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogEmail_ShouldReportDiagnostic()
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
        _logger.LogInformation(""User email: user@example.com"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogCreditCard_ShouldReportDiagnostic()
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
        _logger.LogInformation(""Credit card: 1234-5678-9012-3456"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogSSN_ShouldReportDiagnostic()
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
        _logger.LogInformation(""SSN: 123-45-6789"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogNormalMessage_ShouldNotReportDiagnostic()
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
        public async Task LogInterpolatedStringWithSensitiveInfo_ShouldReportDiagnostic()
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
        var token = ""secret-token-123"";
        _logger.LogInformation($""User token: {token}"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(14, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task ConsoleWriteLineWithSensitiveInfo_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(""Password: mypassword123"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.SensitiveInfoRule)
                .WithLocation(8, 9);

            await VerifyAnalyzerAsync(test, expected);
        }
    }
} 