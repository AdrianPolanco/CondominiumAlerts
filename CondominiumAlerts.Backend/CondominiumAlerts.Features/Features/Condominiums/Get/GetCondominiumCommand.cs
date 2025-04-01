using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;


namespace CondominiumAlerts.Features.Features.Condominiums.Get
{
    public class GetCondominiumCommand : ICommand<Result<GetCondominiumResponse>>
    {
        public string CondominiumId { get; set; } = string.Empty;
    }
}
