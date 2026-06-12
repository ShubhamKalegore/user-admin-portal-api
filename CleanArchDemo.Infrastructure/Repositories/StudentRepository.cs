using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using CleanArchDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchDemo.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllStudents()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student?> GetStudentById(int id)
    {
        return await _context.Students.Where(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Student> Save(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<Student> UpdateStudent(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<bool> DeleteStudent(int id)
    {
        var student = await _context.Students.Where(s => s.Id == id).FirstOrDefaultAsync();
        if (student == null)
        {
            return false;
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Student>> GetStudentsByAge()
    {
        return await _context.Students
            .Where(s => s.Age > 18)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<Student>> GetStudentsWithCourse()
    {
        return await _context.Students
            .Include(s => s.Course)
            .ToListAsync();
    }

    public async Task<List<Student>> GetStudentsByCourseId(int courseId)
    {
        return await _context.Students
            .Where(s => s.CourseId == courseId)
            .Include(s => s.Course)
            .ToListAsync();
    }

    public async Task<List<Course>> GetAllCourses()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<Course> SaveCourse(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        return course;
    }

    public async Task Deletetudent(int id)
    {
        var student = _context.Students.Where(s => s.Id == id).FirstOrDefault();
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
