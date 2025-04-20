using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Join
{
    public class JoinCondominiumCommand : ICommand<Result<JoinCondominiumResponse>>
    {
        public string UserId { get; set; } = string.Empty;
        public string CondominiumCode { get; set; } = string.Empty;
        public string? CondominiumToken { get; set; }

    }
}
