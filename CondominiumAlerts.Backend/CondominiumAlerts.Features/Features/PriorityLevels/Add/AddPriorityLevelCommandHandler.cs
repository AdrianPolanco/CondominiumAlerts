
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Add
{
    public class AddPriorityLevelCommandHandler : ICommandHandler<AddPriorityLevelCommand, Result<AddPriorityLevelResponse>>
    {
        private readonly IRepository<LevelOfPriority, Guid> _levelPepository;
        private readonly IRepository<Condominium, Guid> _codominiumRepository;
        private readonly IValidator<AddPriorityLevelCommand> _validator;
        private readonly ILogger<AddPriorityLevelCommand> _logger;

        public AddPriorityLevelCommandHandler(IRepository<LevelOfPriority, Guid> levelPepository,
            IRepository<Condominium, Guid> codominiumRepository,
            IValidator<AddPriorityLevelCommand> validator,
            ILogger<AddPriorityLevelCommand> logger)
        {
            _levelPepository = levelPepository;
            _codominiumRepository = codominiumRepository;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Result<AddPriorityLevelResponse>> Handle(AddPriorityLevelCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validation = _validator.Validate(request);

            if (!validation.IsValid)
            {
                return validation.ToLightResult<AddPriorityLevelResponse>(_logger);
            }

            if (!await _codominiumRepository.AnyAsync(c => c.Id == request.CondominiumId, cancellationToken))
            {
                _logger.LogWarning("No condominium with the Condominium Id: {request.CondominiumId} was found", request.CondominiumId);
                return Result<AddPriorityLevelResponse>.Fail("No condominium was found");
            }

            LevelOfPriority level = new()
            {
                Title = request.Title,
                Priority = request.Priority,
                Description = request.Description,
                CondominiumId = request.CondominiumId
            };

            level = await _levelPepository.CreateAsync(level, cancellationToken);

            return Result<AddPriorityLevelResponse>.Ok(level);

        }
    }
}
