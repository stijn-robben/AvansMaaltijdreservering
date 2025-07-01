namespace AvansMaaltijdreservering.Core.Domain.Exceptions;

public class ReservationException : BusinessRuleException
{
    public int PackageId { get; }
    public int StudentId { get; }

    public ReservationException(string message, int packageId, int studentId) 
        : base("ReservationRule", message)
    {
        PackageId = packageId;
        StudentId = studentId;
    }

    public ReservationException(string message, int packageId, int studentId, Exception innerException) 
        : base("ReservationRule", message, innerException)
    {
        PackageId = packageId;
        StudentId = studentId;
    }
}