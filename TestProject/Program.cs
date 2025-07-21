using Microsoft.Extensions.Logging;

namespace LoggingAnalyzer.Examples
{
    public class BadLoggingExamples
    {
        private readonly ILogger _logger;

        public BadLoggingExamples(ILogger logger)
        {
            _logger = logger;
        }

        public void Example1_ConsoleWriteLine()
        {
            // LA0001: Console.WriteLine usage
            Console.WriteLine("User logged in successfully");
            Console.Write("Processing request...");
        }

        public void Example2_ImproperExceptionLogging()
        {
            try
            {
                throw new Exception("Something went wrong");
            }
            catch (Exception ex)
            {
                // LA0002: Exception logging without details
                _logger.LogError(ex);
            }
        }

        public void Example3_SensitiveInformationLogging()
        {
            // LA0003: Sensitive information in logs
            _logger.LogInformation("User password: mypassword123");
            _logger.LogInformation("API Key: sk-1234567890abcdef");
            _logger.LogInformation("Email: user@example.com");
            _logger.LogInformation("Credit card: 1234-5678-9012-3456");
            _logger.LogInformation("SSN: 123-45-6789");
        }

        public void Example4_IncorrectLogLevels()
        {
            // LA0004: Incorrect log levels
            _logger.LogInformation("An error occurred during processing");
            _logger.LogError("User logged in successfully");
            _logger.LogInformation("This method is deprecated");
            _logger.LogInformation("Entering method DoSomething");
        }

        public void Example5_StringConcatenation()
        {
            var userId = 123;
            var ipAddress = "192.168.1.1";

            // LA0005: String concatenation in logging
            _logger.LogInformation("User " + userId + " logged in from " + ipAddress);
            _logger.LogInformation(string.Format("User {0} logged in", userId));
            _logger.LogInformation(string.Concat("User ", userId.ToString(), " logged in"));
        }
    }
} 