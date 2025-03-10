

using CondominiumAlerts.Domain.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Users.Update;

public interface IUpdateUserStrategy : IStrategy<UpdateUserCommand, Result<UpdateUserResponse>>
{
    
}