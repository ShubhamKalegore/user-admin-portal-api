using AutoMapper;
using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStudentService _studentService;

        public StudentsController(IMapper mapper, IStudentService studentService) 
        { 
            _mapper = mapper;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentService.GetAllStudents();

            var result = _mapper.Map<List<StudentDto>>(students);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentById(id);

            if (student == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<StudentDto>(student);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(StudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);

            var result = await _studentService.Save(student);

            return Ok(_mapper.Map<StudentDto>(result));
        }

        [HttpPost("course")]
        public async Task<IActionResult> CreateCourse(CourseDto dto)
        {
            var course = _mapper.Map<Course>(dto);

            var result = await _studentService.SaveCourse(course);

            return Ok(_mapper.Map<CourseDto>(result));
        }

        [HttpGet("course")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _studentService.GetAllCourses();

            var result = _mapper.Map<List<CourseDto>>(courses);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, StudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);

            student.Id = id;

            var result = await _studentService.UpdateStudent(student);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StudentDto>(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudent(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("filter-sort")]
        public async Task<IActionResult> GetStudentsByAge()
        {
            var students = await _studentService.GetStudentsByAge();

            return Ok(students);
        }

        [HttpGet("with-course")]
        public async Task<IActionResult> GetStudentsWithCourse()
        {
            var students = await _studentService.GetStudentsWithCourse();

            return Ok(students);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetStudentsByCourseId(int courseId)
        {
            var students = await _studentService.GetStudentsByCourseId(courseId);

            return Ok(students);
        }
    }
}
