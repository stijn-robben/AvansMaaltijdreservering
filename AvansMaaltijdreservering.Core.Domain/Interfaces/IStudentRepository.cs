using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.Domain.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByStudentNumberAsync(string studentNumber);
    Task<Student?> GetByEmailAsync(string email);
    Task<Student> AddAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> StudentNumberExistsAsync(string studentNumber);
    Task<bool> EmailExistsAsync(string email);
}
