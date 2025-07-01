namespace AvansMaaltijdreservering.Core.Domain.Exceptions;

public class StudentBlockedException : BusinessRuleException
{
    public int StudentId { get; }
    public int NoShowCount { get; }

    public StudentBlockedException(int studentId, int noShowCount) 
        : base("StudentBlocked", $"Student {studentId} is blocked due to {noShowCount} no-shows")
    {
        StudentId = studentId;
        NoShowCount = noShowCount;
    }
}