using Application.Products;
using Application.Products.Dtos.Requests;
using Application.Products.Dtos.Responses;
using Domain.Aggregates.ProductAggregate.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Tests.TestDoubles.Mocks;

public class MockProductApplicationService : ProductApplicationService
{
    private readonly List<LogEntry> _loggedMessages;

    public MockProductApplicationService(IProductRepository productRepository) 
        : base(productRepository, null!)
    {
        _loggedMessages = new List<LogEntry>();
        var mockLogger = new MockLogger(_loggedMessages);
        // Reflection to set the logger
        var loggerField = typeof(ProductApplicationService)
            .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        loggerField?.SetValue(this, mockLogger);
    }

    public void VerifyLoggedMessage(string expectedMessage)
    {
        if (!_loggedMessages.Any(entry => entry.Message.Contains(expectedMessage)))
            throw new Exception($"Expected log message containing '{expectedMessage}' was not found");
    }

    public void VerifyLoggedMessageCount(int expectedCount)
    {
        if (_loggedMessages.Count != expectedCount)
            throw new Exception($"Expected {expectedCount} log messages but found {_loggedMessages.Count}");
    }

    public void VerifyLogLevel(string message, LogLevel expectedLevel)
    {
        var entry = _loggedMessages.FirstOrDefault(e => e.Message.Contains(message));
        if (entry == null)
            throw new Exception($"Message containing '{message}' was not found");
        
        if (entry.Level != expectedLevel)
            throw new Exception($"Expected log level {expectedLevel} but found {entry.Level}");
    }

    public record LogEntry(LogLevel Level, string Message);

    private class MockLogger : ILogger<ProductApplicationService>
    {
        private readonly List<LogEntry> _logs;

        public MockLogger(List<LogEntry> logs)
        {
            _logs = logs;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _logs.Add(new LogEntry(logLevel, formatter(state, exception)));
        }
    }
} 