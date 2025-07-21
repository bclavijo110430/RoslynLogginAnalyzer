using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;

namespace LoggingAnalyzer.Tests
{
    public class ExceptionLoggingAnalyzerTests
    {
        private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<LoggingAnalyzer.ExceptionLoggingAnalyzer, DefaultVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }

        [Fact]
        public async Task LogExceptionWithoutDetails_ShouldReportDiagnostic()
        {
            var test = @"
using System;
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
        try
        {
            throw new Exception(""Test exception"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex);
        }
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.ExceptionLoggingRule)
                .WithLocation(20, 13);

            await VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task LogExceptionWithToString_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;
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
        try
        {
            throw new Exception(""Test exception"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogExceptionWithMessage_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;
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
        try
        {
            throw new Exception(""Test exception"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogExceptionWithMessageAndException_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;
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
        try
        {
            throw new Exception(""Test exception"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""An error occurred"");
        }
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task LogNonException_ShouldNotReportDiagnostic()
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
        _logger.LogError(""This is not an exception"");
    }
}";

            await VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task CustomExceptionType_ShouldReportDiagnostic()
        {
            var test = @"
using System;
using Microsoft.Extensions.Logging;

class CustomException : Exception
{
    public CustomException(string message) : base(message) { }
}

class Program
{
    private readonly ILogger _logger;

    public Program(ILogger logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        try
        {
            throw new CustomException(""Test exception"");
        }
        catch (CustomException ex)
        {
            _logger.LogError(ex);
        }
    }
}";

            var expected = new DiagnosticResult(LoggingAnalyzer.Diagnostics.ExceptionLoggingRule)
                .WithLocation(28, 13);

            await VerifyAnalyzerAsync(test, expected);
        }
    }
} 