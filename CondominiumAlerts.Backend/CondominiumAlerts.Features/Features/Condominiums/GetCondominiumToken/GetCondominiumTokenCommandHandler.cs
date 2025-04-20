using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using CondominiumAlerts.Features.Helpers;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Condominiums.GenerateLink;

public class GetCondominiumTokenCommandHandler : ICommandHandler<GetCondominiumTokenCommand,
    Result<GetCondominiumTokenResponse>>
{
    private readonly IRepository<Condominium, Guid> _condominiumRepository;
    private readonly IValidator<GetCondominiumTokenCommand> _validator;
    private readonly ILogger<GetCondominiumTokenResponse> _logger;

    public GetCondominiumTokenCommandHandler(IRepository<Condominium, Guid> condominiumRepository,
        IValidator<GetCondominiumTokenCommand> validator,
        ILogger<GetCondominiumTokenResponse> logger)
    {
        _condominiumRepository = condominiumRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetCondominiumTokenResponse>> Handle(
        GetCondominiumTokenCommand request, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validationResult =
            await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToLightResult<GetCondominiumTokenResponse>(_logger);
        }

        Condominium? condominiumToUse =
            await _condominiumRepository.GetByIdAsync(request.CondominiumId, cancellationToken);

        if (condominiumToUse == null)
        {
            _logger.LogWarning("No condominium was found for the condominium id: {CondominiumId}",
                request.CondominiumId);
            return Result<GetCondominiumTokenResponse>.Fail("No condominium was found");
        }

        string token = string.Empty;
        DateTime expiration;
        if (condominiumToUse?.LinkToken != string.Empty && condominiumToUse.TokenExpirationDate > DateTime.UtcNow)
        {
            token = condominiumToUse.LinkToken;
            expiration = condominiumToUse.TokenExpirationDate;
            return Result<GetCondominiumTokenResponse>.Ok(
                new(token, expiration));
        }

        token = Hasher.HashString(ByteHelpers.GenerateBase64String(10));
        expiration = DateTime.UtcNow.AddDays(14);
        condominiumToUse.LinkToken = token;
        condominiumToUse.TokenExpirationDate = expiration;

        await _condominiumRepository.UpdateAsync(condominiumToUse, cancellationToken);
        
        return Result<GetCondominiumTokenResponse>.Ok(new(token, expiration));
    }
}