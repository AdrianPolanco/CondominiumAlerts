using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;


namespace CondominiumAlerts.Features.Features.Condominium.Join
{
    public class JoinCondominiumCommandHandler : ICommandHandler<JoinCondominiumCommand, Result<JoinCondominiumResponse>>
    {
        private readonly IRepository<Domain.Aggregates.Entities.Condominium, Guid> _condominiumRepository;
        private readonly IRepository<User, string> _userRepository;
        private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
        private readonly IValidator<JoinCondominiumCommand> _validations;
        private readonly ILogger<JoinCondominiumCommand> _logger;

        public JoinCondominiumCommandHandler(
            IRepository<Domain.Aggregates.Entities.Condominium, Guid> condominiumRepository,
            IRepository<User, string> userRepository,
            IRepository<CondominiumUser, Guid> condominiumUserRepository,
            IValidator<JoinCondominiumCommand> validations,
            ILogger<JoinCondominiumCommand> logger
            )
        {
            _condominiumRepository = condominiumRepository;
            _userRepository = userRepository;
            _condominiumUserRepository = condominiumUserRepository;
            _validations = validations;
            _logger = logger;
        }

        public async Task<Result<JoinCondominiumResponse>> Handle(JoinCondominiumCommand request, CancellationToken cancellationToken)
        {
           FluentValidation.Results.ValidationResult validation = _validations.Validate(request);

            if (!validation.IsValid)
            {
                IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
                _logger.LogWarning($"Validation failed {errors}");
                return Result.Fail<JoinCondominiumResponse>(string.Join(", ", errors));
            }

           Domain.Aggregates.Entities.Condominium condominiumTobeJoined = 
                await _condominiumRepository.GetOneByFilterAsync(c => c.InviteCode == request.CondominiumCode);

            if (condominiumTobeJoined == null)
            {
                _logger.LogWarning($"The Code {request.CondominiumCode} dosent belong to any condominium");
                return Result.Fail<JoinCondominiumResponse>($"The Code {request.CondominiumCode} dosent belong to any condominium");
            }

            if (!await _userRepository.AnyAsync((u => u.Id == request.UserId), default))
            {
                _logger.LogWarning($"The user with supposed id {request.UserId} couldn't be founded");
                return Result.Fail<JoinCondominiumResponse>($"The user couldn't be founded");
            }

            if (await _condominiumUserRepository.AnyAsync(cu => cu.UserId == request.UserId && cu.CondominiumId == condominiumTobeJoined.Id, cancellationToken))
            {
                _logger.LogWarning($"The user is already part of the condominium {condominiumTobeJoined?.Name }");
                return Result.Fail<JoinCondominiumResponse>($"The user is already part of the condominium {condominiumTobeJoined?.Name}");
            }

            await _condominiumUserRepository.CreateAsync(new()
            {
                CondominiumId = condominiumTobeJoined.Id,
                UserId = request.UserId,
            }, cancellationToken);

            return Result.Ok<JoinCondominiumResponse>(new(request.UserId, condominiumTobeJoined.Id));

        }
    }
}
