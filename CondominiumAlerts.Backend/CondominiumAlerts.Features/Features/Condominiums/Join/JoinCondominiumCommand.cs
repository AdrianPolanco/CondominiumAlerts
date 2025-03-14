using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Join
{
    public class JoinCondominiumCommand : ICommand<Result<JoinCondominiumResponce>>
    {
        public string UserId { get; set; } = String.Empty;
        public string CondominiumCode { get; set; } = String.Empty;

    }
}
