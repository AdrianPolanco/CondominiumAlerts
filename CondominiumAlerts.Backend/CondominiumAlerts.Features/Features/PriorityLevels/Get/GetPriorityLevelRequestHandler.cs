

using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using MediatR;
using Microsoft.Extensions.Logging;
using CondominiumAlerts.Features.Extensions;
namespace CondominiumAlerts.Features.Features.PriorityLevels.Get
{
    public class GetPriorityLevelRequestHandler : IRequestHandler<GetPriorityLevelsQuery, Result<GetPriorityLevelResponce>>
    {
        private readonly IRepository<LevelOfPriority, Guid> _levelsrepository;
        private readonly IRepository<Condominium, Guid> _condominiumrepository;
        private readonly IValidator<GetPriorityLevelsQuery> _validator;
        private readonly ILogger<GetPriorityLevelRequestHandler> _logger;

        public GetPriorityLevelRequestHandler(IRepository<LevelOfPriority, Guid> levelsrepository,
            IRepository<Condominium, Guid> condominiumrepository,
            IValidator<GetPriorityLevelsQuery> validator,
            ILogger<GetPriorityLevelRequestHandler> logger)
        {
            _levelsrepository = levelsrepository;
            _condominiumrepository = condominiumrepository;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Result<GetPriorityLevelResponce>> Handle(GetPriorityLevelsQuery request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validation = _validator.Validate(request);

            if (!validation.IsValid)
            {
                return validation.ToLightResult<GetPriorityLevelResponce>(_logger);
            }

            if (!await _condominiumrepository.AnyAsync(c => c.Id == request.CondominiumId,cancellationToken))
            {
                _logger.LogWarning("No condominium with the Condominium Id: {request.CondominiumId} was found", request.CondominiumId);
                return Result<GetPriorityLevelResponce>.Fail("No condominium was found");
            }

            // TODO: Change the repository interface to make optional paginations params to the get methods
            List<LevelOfPriority> levels = await _levelsrepository
                      .GetAsync(cancellationToken, l => l.CondominiumId == request.CondominiumId || l.CondominiumId == null);

            if(levels is null)
            {
                _logger.LogWarning("Error while getting the priority levels, with the folowing request {@request}: ", request);
                return Result<GetPriorityLevelResponce>.Fail("No priority levels found");
            }

            if (request.PageNumber > 1)
            {
                return Result<GetPriorityLevelResponce>.Ok(
                  new(request.PageNumber,
                  request.PageSize,
                  levels.Count,
                  levels.Skip((request.PageSize * request.PageNumber) - 1)
                        .Take(request.PageSize).Select(l => (PriorityDto)l).ToList()
                  ));
            }

            return Result<GetPriorityLevelResponce>.Ok(new(request.PageNumber,
                request.PageSize,
                levels.Count,
                levels.Take(request.PageSize).Select(l => (PriorityDto)l).ToList()
                ));

        }
    }
}
