using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Infrastructure.Auth.Interfaces;

public interface IAuthenticationProvider
{
    Task<string> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(string id, string username, CancellationToken cancellationToken);
    Task<string> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
    
    Task<bool> DoesUserExistAsync(string userId, CancellationToken cancellationToken);
}