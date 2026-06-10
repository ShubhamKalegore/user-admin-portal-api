using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Services;

public class StudentService : IStudentService
{
    private readonly ILogger<StudentService> _logger;
    private readonly IStudentRepository _studentRepository;
    public StudentService(ILogger<StudentService> logger, IStudentRepository studentRepository) 
    {
       _logger = logger;
        _studentRepository = studentRepository;
    }

    public async Task<List<Student>> GetAllStudents()
    {
        return await _studentRepository.GetAllStudents();
    }

    public async Task<Student?> GetStudentById(int id)
    {
        return await _studentRepository.GetStudentById(id);
    }

    public async Task<Student> Save(Student student)
    {
        return await _studentRepository.Save(student);
    }

    public async Task<Student?> UpdateStudent(Student student)
    {
        return await _studentRepository.UpdateStudent(student);
    }

    public async Task<bool> DeleteStudent(int id)
    {
        return await _studentRepository.DeleteStudent(id);
    }

    public async Task<List<Student>> GetStudentsByAge()
    {
        return await _studentRepository.GetStudentsByAge();
    }

    public async Task<List<Student>> GetStudentsWithCourse()
    {
        return await _studentRepository.GetStudentsWithCourse();
    }

    public async Task<List<Student>> GetStudentsByCourseId(int courseId)
    {
        return await _studentRepository.GetStudentsByCourseId(courseId);
    }

    public async Task<List<Course>> GetAllCourses()
    {
        return await _studentRepository.GetAllCourses();
    }

    public async Task<Course> SaveCourse(Course course)
    {
        return await _studentRepository.SaveCourse(course);
    }
}
