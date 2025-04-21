using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.GenerateLink;

public class GetCondominiumTokenCommand : ICommand<Result<GetCondominiumTokenResponse>>
{
    public string UserId { get; set; }  = string.Empty;
    public Guid CondominiumId { get; set; }
}