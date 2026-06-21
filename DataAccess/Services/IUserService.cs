using DataAccess.Models;

namespace DataAccess.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string login, string password, CancellationToken cancellationToken = default);
}
