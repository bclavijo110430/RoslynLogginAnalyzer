using System;
using Microsoft.Extensions.Logging;

namespace LoggingAnalyzer.Examples
{
    public class GoodLoggingExamples
    {
        private readonly ILogger _logger;

        public GoodLoggingExamples(ILogger logger)
        {
            _logger = logger;
        }

        public void Example1_StructuredLogging()
        {
            // ✅ Good: Use structured logging instead of Console.WriteLine
            _logger.LogInformation("User logged in successfully");
            _logger.LogInformation("Processing request...");
        }

        public void Example2_ProperExceptionLogging()
        {
            try
            {
                throw new Exception("Something went wrong");
            }
            catch (Exception ex)
            {
                // ✅ Good: Exception logging with details
                _logger.LogError(ex, "An error occurred during processing");
                _logger.LogError("Failed to process request: {Error}", ex.ToString());
                _logger.LogError("Error message: {Message}", ex.Message);
            }
        }

        public void Example3_SafeInformationLogging()
        {
            // ✅ Good: Safe information logging
            _logger.LogInformation("User authentication completed");
            _logger.LogInformation("API request processed successfully");
            _logger.LogInformation("Email notification sent");
            _logger.LogInformation("Payment transaction completed");
            _logger.LogInformation("User profile updated");
        }

        public void Example4_CorrectLogLevels()
        {
            // ✅ Good: Appropriate log levels
            _logger.LogError("An error occurred during processing");
            _logger.LogInformation("User logged in successfully");
            _logger.LogWarning("This method is deprecated");
            _logger.LogDebug("Entering method DoSomething");
            _logger.LogTrace("Internal state: {State}", "some state");
        }

        public void Example5_StructuredLoggingParameters()
        {
            var userId = 123;
            var ipAddress = "192.168.1.1";
            var timestamp = DateTime.UtcNow;

            // ✅ Good: Structured logging parameters
            _logger.LogInformation("User {UserId} logged in from {IPAddress}", userId, ipAddress);
            _logger.LogInformation($"User {userId} logged in at {timestamp:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("Request processed for user {UserId} in {Duration}ms", userId, 150);
        }

        public void Example6_ContextualLogging()
        {
            var correlationId = Guid.NewGuid();
            var userId = 123;
            var operation = "user_login";

            // ✅ Good: Contextual logging with correlation
            _logger.LogInformation("Starting operation {Operation} for user {UserId} with correlation {CorrelationId}", 
                operation, userId, correlationId);

            try
            {
                // Simulate some work
                _logger.LogDebug("Processing authentication for user {UserId}", userId);
                
                // Simulate success
                _logger.LogInformation("Operation {Operation} completed successfully for user {UserId}", 
                    operation, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation {Operation} failed for user {UserId}", 
                    operation, userId);
                throw;
            }
        }

        public void Example7_PerformanceLogging()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                // Simulate some work
                System.Threading.Thread.Sleep(100);
                
                stopwatch.Stop();
                _logger.LogInformation("Database query completed in {Duration}ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Database query failed after {Duration}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
} 