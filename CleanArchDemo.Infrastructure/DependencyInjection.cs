using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Infrastructure.Data;
using CleanArchDemo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            ));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();

        return services;
    }
}