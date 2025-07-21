using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Xunit;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace LoggingAnalyzer.Tests
{
    public class ConsoleWriteLineCodeFixProviderTests
    {
        private static async Task VerifyCodeFixAsync(string source, string expected)
        {
            var test = new CSharpCodeFixTest<LoggingAnalyzer.ConsoleWriteLineAnalyzer, LoggingAnalyzer.ConsoleWriteLineCodeFixProvider, DefaultVerifier>
            {
                TestCode = source,
                FixedCode = expected
            };
            await test.RunAsync();
        }

        [Fact]
        public async Task ConsoleWriteLine_ShouldBeReplacedWithStructuredLogging()
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

            var expected = @"
using System;

class Program
{
    static void Main()
    {
        _logger.LogInformation(""Hello World"");
    }
}";

            await VerifyCodeFixAsync(test, expected);
        }

        [Fact]
        public async Task ConsoleWrite_ShouldBeReplacedWithStructuredLogging()
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

            var expected = @"
using System;

class Program
{
    static void Main()
    {
        _logger.LogInformation(""Hello World"");
    }
}";

            await VerifyCodeFixAsync(test, expected);
        }

        [Fact]
        public async Task ConsoleWriteLineWithFormat_ShouldBeReplacedWithStructuredLogging()
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

            var expected = @"
using System;

class Program
{
    static void Main()
    {
        _logger.LogInformation(""Hello {0}"", ""World"");
    }
}";

            await VerifyCodeFixAsync(test, expected);
        }

        [Fact]
        public async Task ConsoleWriteLineWithMultipleArguments_ShouldBeReplacedWithStructuredLogging()
        {
            var test = @"
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(""User {0} logged in at {1}"", ""John"", DateTime.Now);
    }
}";

            var expected = @"
using System;

class Program
{
    static void Main()
    {
        _logger.LogInformation(""User {0} logged in at {1}"", ""John"", DateTime.Now);
    }
}";

            await VerifyCodeFixAsync(test, expected);
        }
    }
} 