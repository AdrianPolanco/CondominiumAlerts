namespace CondominiumAlerts.Infrastructure.Auth.Interfaces;

public interface IAuthenticationProvider
{
    Task<string> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken);
    Task<string> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
}