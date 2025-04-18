using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;


namespace CondominiumAlerts.Features.Features.Condominiums.GetCondominiumsJoinedByUser
{
    public class GetCondominiumsJoinedByUserCommand : ICommand<Result<List<GetCondominiumsJoinedByUserResponse>>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
