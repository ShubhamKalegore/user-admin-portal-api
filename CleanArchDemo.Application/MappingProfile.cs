using AutoMapper;
using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Domain.Entities;

namespace CleanArchDemo.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();

        CreateMap<CreateProductDto, Product>();
        CreateMap<Product, CreateProductDto>();

        CreateMap<StudentDto, Student>()
            .ForMember(dest => dest.Course, opt => opt.Ignore());

        CreateMap<Student, StudentDto>()
            .ForMember(
                dest => dest.Course,
                opt => opt.MapFrom(src => src.Course == null ? string.Empty : src.Course.Name));

        CreateMap<CourseDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Course, CourseDto>();
        
    }
}
