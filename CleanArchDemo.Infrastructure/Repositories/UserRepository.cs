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

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync() =>
        await _context.Users.AsNoTracking().ToListAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.FindAsync(id);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.RefreshToken == refreshToken);
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteUser(User user)
    {
        _context.Users.Remove(user);
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<User> UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);
}
