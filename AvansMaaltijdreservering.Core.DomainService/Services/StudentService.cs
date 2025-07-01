using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        return await _studentRepository.GetByIdAsync(studentId);
    }

    public async Task<Student?> GetStudentByStudentNumberAsync(string studentNumber)
    {
        return await _studentRepository.GetByStudentNumberAsync(studentNumber);
    }

    public async Task<Student> RegisterStudentAsync(Student student)
    {
        await ValidateStudentAgeAsync(student);
        await ValidateStudentUniquenessAsync(student);
        
        return await _studentRepository.AddAsync(student);
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(student.Id);
        if (existingStudent == null)
            throw new ArgumentException("Student not found");

        // Check uniqueness only if student number or email changed
        if (existingStudent.StudentNumber != student.StudentNumber || 
            existingStudent.Email != student.Email)
        {
            await ValidateStudentUniquenessAsync(student);
        }

        await ValidateStudentAgeAsync(student);
        
        return await _studentRepository.UpdateAsync(student);
    }

    public async Task ValidateStudentAgeAsync(Student student)
    {
        if (student.DateOfBirth > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future");

        if (student.GetAge() < 16)
            throw new ArgumentException("Student must be at least 16 years old to register");
    }

    public async Task ValidateStudentUniquenessAsync(Student student)
    {
        if (await _studentRepository.StudentNumberExistsAsync(student.StudentNumber))
            throw new InvalidOperationException("Student number already exists");

        if (await _studentRepository.EmailExistsAsync(student.Email))
            throw new InvalidOperationException("Email address already exists");
    }

    public async Task<bool> CanStudentMakeReservationAsync(int studentId, DateTime pickupDate)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            return false;

        if (student.IsBlocked())
            return false;

        return !student.HasReservationOnDate(pickupDate);
    }

    public async Task RegisterNoShowAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new ArgumentException("Student not found");

        student.NoShowCount++;
        await _studentRepository.UpdateAsync(student);
    }

    public async Task<bool> IsStudentBlockedAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        return student?.IsBlocked() ?? false;
    }
}
