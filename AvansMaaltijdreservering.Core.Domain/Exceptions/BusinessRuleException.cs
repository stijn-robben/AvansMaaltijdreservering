namespace AvansMaaltijdreservering.Core.Domain.Exceptions;

public class BusinessRuleException : Exception
{
    public string RuleName { get; }
    public object? Context { get; }

    public BusinessRuleException(string ruleName, string message) : base(message)
    {
        RuleName = ruleName;
    }

    public BusinessRuleException(string ruleName, string message, object? context) : base(message)
    {
        RuleName = ruleName;
        Context = context;
    }

    public BusinessRuleException(string ruleName, string message, Exception innerException) : base(message, innerException)
    {
        RuleName = ruleName;
    }
}