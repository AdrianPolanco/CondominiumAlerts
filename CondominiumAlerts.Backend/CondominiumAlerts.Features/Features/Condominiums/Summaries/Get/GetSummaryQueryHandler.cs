using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries.Get;

public class GetSummaryQueryHandler : IQueryHandler<GetSummaryQuery, Result<GetSummaryResponse>>
{
    private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
    private readonly IRepository<Summary, Guid> _summaryRepository;

    public GetSummaryQueryHandler(IRepository<CondominiumUser, Guid> condominiumUserRepository, IRepository<Summary, Guid> summaryRepository)
    {
        _condominiumUserRepository = condominiumUserRepository;
        _summaryRepository = summaryRepository;
    }
    
    public async Task<Result<GetSummaryResponse>> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
    {
        var condominiumUser = await _condominiumUserRepository.GetAsync(
            cancellationToken: cancellationToken,
            filter: cu => cu.CondominiumId == request.CondominiumId && cu.UserId == request.UserId
            );

        if (condominiumUser.Count == 0) return Result.Fail<GetSummaryResponse>("El condominio no existe o el usuario no se encuentra en el condominio.");

        var summaries = await _summaryRepository.GetAsync(
            cancellationToken: cancellationToken,
            filter: s => s.CondominiumId == request.CondominiumId 
                         && s.CreatedAt >= DateTime.UtcNow.AddHours(-24) 
                         && s.CreatedAt < DateTime.UtcNow
            );

        var summary = summaries.LastOrDefault();
        
        return Result.Ok<GetSummaryResponse>(new GetSummaryResponse(summary));
    }
}