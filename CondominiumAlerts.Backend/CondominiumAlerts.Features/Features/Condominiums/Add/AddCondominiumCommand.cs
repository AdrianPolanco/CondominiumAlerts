using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Condominiums.Add
{
    public class AddCondominiumCommand
    : ICommand<Result<AddCondominiumResponse>>
    {
        public Guid Id { get; set; }
        public string userId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public required IFormFile ImageFile { get; set; }
    }
}