using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;
using CondominiumEntity = CondominiumAlerts.Domain.Aggregates.Entities.Condominium;
namespace CondominiumAlerts.Features.Features.Condominium.Get
{
    public class GetCondominiumCommandHandler : ICommandHandler<GetCondominiumCommand, Result<GetCondominiumResponce>>
    {
        private readonly IRepository<CondominiumEntity, Guid> _condominiumRepository;
        private readonly IValidator<GetCondominiumCommand> _validator;
        private readonly ILogger<GetCondominiumCommand> _logger;

        public GetCondominiumCommandHandler( 
            IRepository<CondominiumEntity, Guid> condominiumRepository,
            IValidator<GetCondominiumCommand> validator,
            ILogger<GetCondominiumCommand> logger
            )
        {
            _condominiumRepository = condominiumRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<GetCondominiumResponce>> Handle(GetCondominiumCommand request, CancellationToken cancellationToken)
        {
         FluentValidation.Results.ValidationResult validResult = _validator.Validate(request);   
            if(!validResult.IsValid)
            {
                IEnumerable<string> erros = validResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogWarning("validation failed: \n {errors}", String.Join("\n ", erros));
                return Result.Fail<GetCondominiumResponce>(String.Join(", ", erros)); 
            }

            CondominiumEntity condominium = 
                await _condominiumRepository.GetByIdAsync(Guid.Parse(request.CondominiumId), cancellationToken);

            if (condominium == null) {
                _logger.LogWarning("No condominium with the id {request.CondominiumId} was found", request.CondominiumId);
                return Result.Fail<GetCondominiumResponce>("No condominium was found");
            }

            return Result< GetCondominiumResponce >.Ok(new(
                condominium.Id,
                condominium.Name,
                condominium.Address, 
                condominium.ImageUrl,
                condominium.InviteCode, 
                condominium.LinkToken,
                condominium.TokenExpirationDate,
                condominium.Users == null ? condominium.Users.Count : 0));
        }
    }
}
