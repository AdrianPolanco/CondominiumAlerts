using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominium.Join
{
    public class JoinCondominiumCommand : ICommand<Result<JoinCondominiumResponse>>
    {
        public string UserId { get; set; } = String.Empty;
        public string CondominiumCode { get; set; } = String.Empty;

    }
}
