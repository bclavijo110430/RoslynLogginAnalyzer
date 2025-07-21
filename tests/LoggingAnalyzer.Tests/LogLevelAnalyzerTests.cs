using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;

namespace LoggingAnalyzer.Tests
{
    public class LogLevelAnalyzerTests
    {
        private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<LoggingAnalyzer.LogLevelAnalyzer, DefaultVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }

        [Fact]
        public async Task LogErrorWithErrorKeywords_ShouldNotReportDiagnostic()
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
        _logger.LogError(""An error occurred during processing"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogInfoWithErrorKeywords_ShouldReportDiagnostic()
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
        _logger.LogInformation(""An error occurred during processing"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.IncorrectLogLevelRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogWarningWithWarningKeywords_ShouldNotReportDiagnostic()
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
        _logger.LogWarning(""This method is deprecated"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogInfoWithWarningKeywords_ShouldReportDiagnostic()
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
        _logger.LogInformation(""This method is deprecated"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.IncorrectLogLevelRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogDebugWithDebugKeywords_ShouldNotReportDiagnostic()
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
        _logger.LogDebug(""Entering method DoSomething"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogInfoWithDebugKeywords_ShouldReportDiagnostic()
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
        _logger.LogInformation(""Entering method DoSomething"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.IncorrectLogLevelRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogTraceWithTraceKeywords_ShouldNotReportDiagnostic()
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
        _logger.LogTrace(""Internal state: {state}"", ""some state"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogInfoWithTraceKeywords_ShouldReportDiagnostic()
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
        _logger.LogInformation(""Internal state: {state}"", ""some state"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.IncorrectLogLevelRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogInfoWithNormalMessage_ShouldNotReportDiagnostic()
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
        public async Task LogErrorWithNormalMessage_ShouldReportDiagnostic()
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
        _logger.LogError(""User logged in successfully"");
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.IncorrectLogLevelRule)
                .WithLocation(13, 9);

            await VerifyAnalyzerAsync(test, expected);
        }
    }
} 