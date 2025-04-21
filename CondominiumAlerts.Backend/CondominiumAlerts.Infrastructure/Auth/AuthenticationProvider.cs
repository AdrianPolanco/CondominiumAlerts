using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using FirebaseAdmin.Auth;

namespace CondominiumAlerts.Infrastructure.Auth;

public class AuthenticationProvider : IAuthenticationProvider
{
    private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
    public AuthenticationProvider(IRepository<CondominiumUser, Guid> condominiumUserRepository)
    {
        _condominiumUserRepository = condominiumUserRepository;
    }
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

    public bool DoesHavePermission<T>(T id1, T id2)
    {
        if (typeof(T) != typeof(Guid) && typeof(T) != typeof(string))
            throw new ArgumentException("T debe ser Guid o string");

        return Equals(id1, id2);
    }


    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(userId, cancellationToken);
    }

    public async Task<bool> DoesUserExistAsync(string userId, CancellationToken cancellationToken)
    {
        return await FirebaseAuth.DefaultInstance.GetUserAsync(userId, cancellationToken) != null;
    }

    public async Task<bool> IsUserInCondominiumAsync(string userId, Guid condominiumId, CancellationToken cancellationToken)
    {
        var condominiumUsers = await _condominiumUserRepository.GetAsync(
            cancellationToken,
            filter => filter.UserId == userId && filter.CondominiumId == condominiumId);
        
        var condominiumUser = condominiumUsers.FirstOrDefault();
        
        return condominiumUser != null;
    }
}