using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchDemo.Domain.Entities;

namespace CleanArchDemo.Application.Interfaces;

public interface IStudentRepository
{
    Task<List<Student>> GetAllStudents();

    Task<Student?> GetStudentById(int id);

    Task<Student> Save(Student student);

    Task<Student?> UpdateStudent(Student student);

    Task<bool> DeleteStudent(int id);

    Task<List<Student>> GetStudentsByAge();

    Task<List<Student>> GetStudentsWithCourse();

    Task<List<Student>> GetStudentsByCourseId(int courseId);

    Task<List<Course>> GetAllCourses();

    Task<Course> SaveCourse(Course course);
}
