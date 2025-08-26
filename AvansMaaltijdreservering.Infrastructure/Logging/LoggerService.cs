using Microsoft.Extensions.Logging;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Infrastructure.Logging;

public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    public void LogInfo(string message)
    {
        _logger.LogInformation("{Message}", message);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning("{Message}", message);
    }

    public void LogError(string message, Exception? exception = null)
    {
        if (exception != null)
            _logger.LogError(exception, "{Message}", message);
        else
            _logger.LogError("{Message}", message);
    }

    public void LogCritical(string message, Exception? exception = null)
    {
        if (exception != null)
            _logger.LogCritical(exception, "{Message}", message);
        else
            _logger.LogCritical("{Message}", message);
    }
}