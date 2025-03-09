using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;


namespace CondominiumAlerts.Features.Features.Condominium.Get
{
    public class GetCondominiumCommand : ICommand<Result<GetCondominiumResponce>>
    {
        public string CondominiumId { get; set; } = string.Empty;
    }
}
