

using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using FluentValidation;
using LightResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.PriorityLevels.GetById
{
    public class GetByIdPriorityLevelQueryHandler : IRequestHandler<GetByIdPriorityLevelQuery, Result<GetByIdPriorityLevelResponse>>
    {
        private readonly IRepository<LevelOfPriority, Guid> _priorityLevelRepository;
        private readonly ILogger<GetByIdPriorityLevelQuery> _logger;
        private readonly IValidator<GetByIdPriorityLevelQuery> _validator;

        public GetByIdPriorityLevelQueryHandler(
            IRepository<LevelOfPriority, Guid> priorityLevelRepository,
            ILogger<GetByIdPriorityLevelQuery> logger,
            IValidator<GetByIdPriorityLevelQuery> validator
            )
        {
            _priorityLevelRepository = priorityLevelRepository;
            _logger = logger;
            _validator = validator;
        }
        public async Task<Result<GetByIdPriorityLevelResponse>> Handle(GetByIdPriorityLevelQuery request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.ToLightResult<GetByIdPriorityLevelResponse>(_logger);
            }

            LevelOfPriority? levelOfPriority = await _priorityLevelRepository
                .GetOneByFilterAsync(l => l.Id == request.Id && l.CondominiumId == request.CondominiumId, cancellationToken);

            if (levelOfPriority is null)
            {
                _logger.LogWarning("No level with the Id: {request.Id} and belonging to the condominium with the Id:{CondominiumId} was found", request.Id, request.CondominiumId);
                return Result<GetByIdPriorityLevelResponse>.Fail("No Priority level was found");
            }

            return Result<GetByIdPriorityLevelResponse>.Ok(levelOfPriority);
        }
    }
}
