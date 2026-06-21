using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services;

public sealed class UserService : IUserService
{
    private readonly AppDatabaseContext _context;

    public UserService(AppDatabaseContext context) => _context = context;

    public async Task<User?> AuthenticateAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Login == login && user.Password == password,
                cancellationToken);
    }
}
