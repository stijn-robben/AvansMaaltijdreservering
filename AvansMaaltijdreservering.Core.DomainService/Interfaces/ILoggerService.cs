namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface ILoggerService
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
    void LogCritical(string message, Exception? exception = null);
}