using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IStudentService
{
    Task<Student?> GetStudentByIdAsync(int id);
    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber);
    Task<Student?> GetStudentByEmailAsync(string email);
    
    Task<Student> RegisterStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student);
    
    Task<bool> IsStudentBlockedAsync(int studentId);
    Task<bool> IsStudentEligibleForPackageAsync(int studentId, Package package);
    Task<bool> CanReservePackageAsync(int studentId, Package package);
}