using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.Domain.Exceptions;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILoggerService _logger;

    public StudentService(IStudentRepository studentRepository, ILoggerService logger)
    {
        _studentRepository = studentRepository;
        _logger = logger;
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }

    public async Task<Student?> GetStudentByStudentNumberAsync(string studentNumber)
    {
        return await _studentRepository.GetByStudentNumberAsync(studentNumber);
    }

    public async Task<Student?> GetStudentByEmailAsync(string email)
    {
        return await _studentRepository.GetByEmailAsync(email);
    }

    public async Task<Student> RegisterStudentAsync(Student student)
    {
        if (await _studentRepository.StudentNumberExistsAsync(student.StudentNumber))
            throw new ArgumentException("Student number already exists");

        if (await _studentRepository.EmailExistsAsync(student.Email))
            throw new ArgumentException("Email already exists");

        if (student.GetAge() < 16)
            throw new ArgumentException("Student must be at least 16 years old");

        _logger.LogInfo($"Registering new student: {student.StudentNumber}");
        return await _studentRepository.AddAsync(student);
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        var existing = await _studentRepository.GetByIdAsync(student.Id);
        if (existing == null)
            throw new ArgumentException("Student not found");

        _logger.LogInfo($"Updating student: {student.StudentNumber}");
        return await _studentRepository.UpdateAsync(student);
    }

    public async Task<bool> IsStudentBlockedAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        return student?.IsBlocked() ?? false;
    }

    public async Task<bool> CanReservePackageAsync(int studentId, Package package)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null) return false;

        if (student.IsBlocked())
            return false;

        if (package.Is18Plus && !student.IsAdult())
            return false;

        if (student.HasReservationOnDate(package.PickupTime.Date))
            return false;

        return true;
    }
}