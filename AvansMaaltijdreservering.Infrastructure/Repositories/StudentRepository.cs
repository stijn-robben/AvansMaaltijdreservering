using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Data;

namespace AvansMaaltijdreservering.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.Reservations)
            .ThenInclude(p => p.Products)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(s => s.Reservations)
            .ThenInclude(p => p.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByStudentNumberAsync(string studentNumber)
    {
        return await _context.Students
            .Include(s => s.Reservations)
            .ThenInclude(p => p.Products)
            .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _context.Students
            .Include(s => s.Reservations)
            .ThenInclude(p => p.Products)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student> AddAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> StudentNumberExistsAsync(string studentNumber)
    {
        return await _context.Students.AnyAsync(s => s.StudentNumber == studentNumber);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Students.AnyAsync(s => s.Email == email);
    }
}
