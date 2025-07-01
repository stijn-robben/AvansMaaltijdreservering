using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IStudentService
{
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber);
    Task<Student> RegisterStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student);
    Task ValidateStudentAgeAsync(Student student);
    Task ValidateStudentUniquenessAsync(Student student);
    Task<bool> CanStudentMakeReservationAsync(int studentId, DateTime pickupDate);
    Task RegisterNoShowAsync(int studentId);
    Task<bool> IsStudentBlockedAsync(int studentId);
}
