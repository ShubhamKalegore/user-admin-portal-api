using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Application.Services;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchDemo.Application.Tests.Services;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _repositoryMock = new();
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _service = new StudentService(
            Mock.Of<ILogger<StudentService>>(),
            _repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnStudents()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Amit", Email = "amit@test.com", Age = 21 },
            new() { Id = 2, Name = "Neha", Email = "neha@test.com", Age = 22 }
        };

        _repositoryMock
            .Setup(repository => repository.GetAllStudents())
            .ReturnsAsync(students);

        var result = await _service.GetAllStudents();

        Assert.Equal(2, result.Count);
        Assert.Equal("Amit", result[0].Name);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnStudent_WhenStudentExists()
    {
        var student = new Student
        {
            Id = 1,
            Name = "Rahul",
            Email = "rahul@test.com",
            Age = 20
        };

        _repositoryMock
            .Setup(repository => repository.GetStudentById(1))
            .ReturnsAsync(student);

        var result = await _service.GetStudentById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Rahul", result.Name);
    }

    [Fact]
    public async Task Save_ShouldCallRepositoryAndReturnSavedStudent()
    {
        var student = new Student
        {
            Name = "Priya",
            Email = "priya@test.com",
            Age = 19
        };

        var savedStudent = new Student
        {
            Id = 5,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age
        };

        _repositoryMock
            .Setup(repository => repository.Save(student))
            .ReturnsAsync(savedStudent);

        var result = await _service.Save(student);

        Assert.Equal(5, result.Id);
        Assert.Equal("Priya", result.Name);
    }

    [Fact]
    public async Task UpdateStudent_ShouldCallRepositoryAndReturnUpdatedStudent()
    {
        var student = new Student
        {
            Id = 3,
            Name = "Updated Name",
            Email = "updated@test.com",
            Age = 23
        };

        _repositoryMock
            .Setup(repository => repository.UpdateStudent(student))
            .ReturnsAsync(student);

        var result = await _service.UpdateStudent(student);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnRepositoryResult()
    {
        _repositoryMock
            .Setup(repository => repository.DeleteStudent(4))
            .ReturnsAsync(true);

        var result = await _service.DeleteStudent(4);

        Assert.True(result);
    }

    [Fact]
    public async Task GetStudentsByCourseId_ShouldReturnStudentsForCourse()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Kiran", CourseId = 2 },
            new() { Id = 2, Name = "Meera", CourseId = 2 }
        };

        _repositoryMock
            .Setup(repository => repository.GetStudentsByCourseId(2))
            .ReturnsAsync(students);

        var result = await _service.GetStudentsByCourseId(2);

        Assert.Equal(2, result.Count);
        Assert.All(result, student => Assert.Equal(2, student.CourseId));
    }
}
