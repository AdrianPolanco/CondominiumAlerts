
using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;


namespace CondominiumAlerts.Features.Features.Users.GetCondominiumsUsers
{
    public class GetCondominiumsUsersCommand : ICommand<Result<List<GetCondominiumsUsersResponse>>>
    {
        public string CondominiumId { get; set; } = String.Empty;
    }
}
