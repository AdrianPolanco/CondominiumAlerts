using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using FirebaseAdmin.Auth;

namespace CondominiumAlerts.Infrastructure.Auth;

public class AuthenticationProvider : IAuthenticationProvider
{
    public async Task<string> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken)
    {
        var userArgs = new UserRecordArgs { DisplayName = username, Email = email, Password = password };
        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs, cancellationToken);
        return userRecord.Uid;
    }

    public async Task<bool> UpdateUserAsync(string id, string username, CancellationToken cancellationToken)
    {
        var userArgs = new UserRecordArgs { Uid = id, DisplayName = username };
        var result = await FirebaseAuth.DefaultInstance.UpdateUserAsync(userArgs, cancellationToken);
        return result.DisplayName == username;
    }

    public async Task<string> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(userId, cancellationToken);
    }

    public async Task<bool> DoesUserExistAsync(string userId, CancellationToken cancellationToken)
    {
        return await FirebaseAuth.DefaultInstance.GetUserAsync(userId, cancellationToken) != null;
    }
}