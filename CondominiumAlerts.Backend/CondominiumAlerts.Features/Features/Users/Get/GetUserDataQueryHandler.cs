using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Users;

public class GetUserDataQueryHandler : IQueryHandler<GetUserDataQuery, Result<GetUserDataResponse>>
{
    private readonly IRepository<User, string> _userRepository;

    public GetUserDataQueryHandler(IRepository<User, string> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<GetUserDataResponse>> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken, true);
        var response = user.Adapt<GetUserDataResponse>();
        
        return Result<GetUserDataResponse>.Ok(response);
    }
}