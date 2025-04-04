
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Delete
{
    public class DeletePriorityLevelCommandHandler : ICommandHandler<DeletePriorityLevelCommand, Result<DeletePriorityLevelResponse>>
    {
        private readonly IRepository<LevelOfPriority, Guid> _priorityLevelRepository;
        private readonly IRepository<Condominium, Guid> _condominiumRepository;
        private readonly ILogger<DeletePriorityLevelCommand> _logger;
        private readonly IValidator<DeletePriorityLevelCommand> _validator;

        public DeletePriorityLevelCommandHandler(
            IRepository<LevelOfPriority, Guid> priorityLevelRepository,
            IRepository<Condominium, Guid> condominiumRepository,
            ILogger<DeletePriorityLevelCommand> logger,
            IValidator<DeletePriorityLevelCommand> validator
            )
        {
            _priorityLevelRepository = priorityLevelRepository;
            _condominiumRepository = condominiumRepository;
            _logger = logger;
            _validator = validator;
        }
        public async Task<Result<DeletePriorityLevelResponse>> Handle(DeletePriorityLevelCommand request, CancellationToken cancellationToken)
        {
           FluentValidation.Results.ValidationResult validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.ToLightResult<DeletePriorityLevelResponse>();
            }

            if(!await _priorityLevelRepository.AnyAsync(l => l.Id == request.Id && l.CondominiumId == request.CondominiumId, cancellationToken))
            {
                _logger.LogWarning("No level with the Id: {request.Id} and belonging to the condominium with the Id:{CondominiumId} was found", request.Id, request.CondominiumId);
                return Result<DeletePriorityLevelResponse>.Fail("No Priority level was found");
            }

            LevelOfPriority? levelOfPriority =  await _priorityLevelRepository.DeleteAsync(request.Id, cancellationToken);

            if (levelOfPriority == null)
            {
                _logger.LogWarning("Error while deleting the level with the folowing request: {@request} ", request);
                return Result<DeletePriorityLevelResponse>.Fail("Error while deleting the Priority level");
            }

            return Result<DeletePriorityLevelResponse>.Ok(levelOfPriority);
        }
    }
}
