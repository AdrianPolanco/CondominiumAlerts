using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;


namespace CondominiumAlerts.Features.Features.Condominium.GetCondominiumsJoinedByUser
{
    public class GetCondominiumsJoinedByUserCommand : ICommand<Result<List<GetCondominiumsJoinedByUserResponse>>>
    {
        public string UserId { get; set; } = String.Empty;
    }
}
