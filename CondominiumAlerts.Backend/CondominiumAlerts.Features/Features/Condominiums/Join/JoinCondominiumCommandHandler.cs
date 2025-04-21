using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;


namespace CondominiumAlerts.Features.Features.Condominiums.Join
{
    public class
        JoinCondominiumCommandHandler : ICommandHandler<JoinCondominiumCommand, Result<JoinCondominiumResponse>>
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

        public async Task<Result<JoinCondominiumResponse>> Handle(JoinCondominiumCommand request,
            CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validationResult
                = await _validations.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return validationResult.ToLightResult<JoinCondominiumResponse>(_logger);
            }

            if (request.CondominiumToken is not "" or null)
            {
                return await JoinWithToken(request, cancellationToken);
            }

            return await JoinWithCode(request, cancellationToken);
        }

        private async Task<Result<JoinCondominiumResponse>> JoinWithToken(JoinCondominiumCommand request,
            CancellationToken cancellationToken)
        {
            if (request.CondominiumToken is "" or null)
            {
                _logger.LogWarning("The token was null is expired");
                return Result<JoinCondominiumResponse>.Fail("the link is null");
            }
            
            Condominium? condominiumToBeJoined =
                await _condominiumRepository.GetOneByFilterAsync(c => c.LinkToken == request.CondominiumToken,
                    cancellationToken);
            if (condominiumToBeJoined is null)
            {
                _logger.LogWarning("No condominium found for the Token {CondominiumToken}", request.CondominiumToken);
                return Result<JoinCondominiumResponse>.Fail("The link does not belong to any condominium");
            }

            if (condominiumToBeJoined.TokenExpirationDate < DateTime.UtcNow)
            {
                _logger.LogWarning("The token {CondominiumToken} is expired", request.CondominiumToken);
                return Result<JoinCondominiumResponse>.Fail("The link is expired");
            }

            if (!await _userRepository.AnyAsync(u => u.Id == request.UserId, cancellationToken))
            {
                _logger.LogWarning("The user with supposed Id {UserId} couldn't be founded", request.UserId);
                return Result.Fail<JoinCondominiumResponse>($"The user couldn't be founded");
            }

            if (await _condominiumUserRepository.AnyAsync(
                    u => u.UserId == request.UserId && u.CondominiumId == condominiumToBeJoined.Id, cancellationToken))
            {
                _logger.LogWarning("The user is already part of the condominium {Name}", condominiumToBeJoined?.Name);
                return Result.Fail<JoinCondominiumResponse>(
                    $"The user is already part of the condominium {condominiumToBeJoined?.Name}");
            }

            await _condominiumUserRepository.CreateAsync(new()
            {
                CondominiumId = condominiumToBeJoined.Id,
                UserId = request.UserId,
            },cancellationToken);

            return Result<JoinCondominiumResponse>.Ok(new(request.UserId, condominiumToBeJoined.Id));
        }

        private async Task<Result<JoinCondominiumResponse>> JoinWithCode(JoinCondominiumCommand request,
            CancellationToken cancellationToken)
        {
            Domain.Aggregates.Entities.Condominium? condominiumToBeJoined =
                await _condominiumRepository.GetOneByFilterAsync(c => c.InviteCode == request.CondominiumCode,
                    cancellationToken);

            if (condominiumToBeJoined == null)
            {
                _logger.LogWarning("The Code {CondominiumCode} doesn't belong to any condominium",
                    request.CondominiumCode);
                return Result.Fail<JoinCondominiumResponse>(
                    $"The Code {request.CondominiumCode} doesn't belong to any condominium");
            }

            if (!await _userRepository.AnyAsync((u => u.Id == request.UserId), cancellationToken))
            {
                _logger.LogWarning("The user with supposed Id {UserId} couldn't be founded", request.UserId);
                return Result.Fail<JoinCondominiumResponse>($"The user couldn't be founded");
            }

            if (await _condominiumUserRepository.AnyAsync(
                    cu => cu.UserId == request.UserId && cu.CondominiumId == condominiumToBeJoined.Id,
                    cancellationToken))
            {
                _logger.LogWarning("The user is already part of the condominium {Name}", condominiumToBeJoined?.Name);
                return Result.Fail<JoinCondominiumResponse>(
                    $"The user is already part of the condominium {condominiumToBeJoined?.Name}");
            }

            await _condominiumUserRepository.CreateAsync(new()
            {
                CondominiumId = condominiumToBeJoined.Id,
                UserId = request.UserId,
            }, cancellationToken);

            return Result<JoinCondominiumResponse>.Ok(new(request.UserId, condominiumToBeJoined.Id));
        }
    }
}