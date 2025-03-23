
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Update
{
    public class UpdatePriorityLevelCommandHandler : ICommandHandler<UpdatePriorityLevelCommand, Result<UpdatePriorityLevelResponse>>
    {
        private readonly IRepository<LevelOfPriority, Guid> _levelRepository;
        private readonly ILogger<UpdatePriorityLevelCommand> _logger;
        private readonly IValidator<UpdatePriorityLevelCommand> _validator;

        public UpdatePriorityLevelCommandHandler(IRepository<LevelOfPriority, Guid> levelRepository, 
            ILogger<UpdatePriorityLevelCommand> logger,
            IValidator<UpdatePriorityLevelCommand> validator)
        {
            _levelRepository = levelRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result<UpdatePriorityLevelResponse>> Handle(UpdatePriorityLevelCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validation = _validator.Validate(request);

            if (!validation.IsValid)
            {
                return validation.ToLightResult<UpdatePriorityLevelResponse>(_logger);
            }

            LevelOfPriority? level = await _levelRepository.GetByIdAsync(request.Id, cancellationToken);

            if(level is null)
            {
                _logger.LogWarning("No level with the Id: {request.Id} was found", request.Id);
                return Result<UpdatePriorityLevelResponse>.Fail("No Priority level was found ");
            }

            level.Title = request.Title;
            level.Priority = request.Priority;
            level.Description = request.Description;

            level = await _levelRepository.UpdateAsync(level, cancellationToken);

            if (level == null)
            {
                _logger.LogWarning("Error while updating the level with the folowing request {@request}: ", request);
                return Result<UpdatePriorityLevelResponse>.Fail("Error while updating the Priority level");
            }

            return Result<UpdatePriorityLevelResponse>.Ok(level);
        }
    }
}
