using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Helpers;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Features.Extensions;

namespace CondominiumAlerts.Features.Features.Condominiums.Add
{
    public class AddCondominiumHandler
    : ICommandHandler<AddCondominiumCommand, Result<AddCondominiumResponse>>
    {
        private readonly Cloudinary _cloudinary;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly ILogger<AddCondominiumHandler> _logger;
        private readonly IValidator<AddCondominiumCommand> _validator;
        private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;

        public AddCondominiumHandler(Cloudinary cloudinary,
                                     ICondominiumRepository condominiumRepository,
                                     ILogger<AddCondominiumHandler> logger,
                                     IValidator<AddCondominiumCommand> validator,
                                     IRepository<CondominiumUser, Guid>
                                         condominiumUserRepository)
        {
            _cloudinary = cloudinary;
            _condominiumRepository = condominiumRepository;
            _logger = logger;
            _validator = validator;
            _condominiumUserRepository = condominiumUserRepository;
        }

        public async Task<Result<AddCondominiumResponse>>
        Handle(AddCondominiumCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validation = await 
                _validator.ValidateAsync(request,cancellationToken);

            if (!validation.IsValid)
            {
                /*IEnumerable< string> errors = validation.Errors.Select(e => e.ErrorMessage);
                _logger.LogTrace($"Validation failed {errors}");*/
                return validation.ToLightResult<AddCondominiumResponse>(_logger);
            }

            ImageUploadResult imageUploadResult = await _cloudinary.UploadAsync(new ImageUploadParams()
            {
                File = new FileDescription(Guid.NewGuid().ToString(), request.ImageFile.OpenReadStream()),
            });
            if (imageUploadResult.Error?.Message is { } message) {
                _logger.LogTrace("Failed to upload image, error with message {Error}.", message);
                return Result.Fail<AddCondominiumResponse>(message);
            }

            string inviteCode = ByteHelpers.GenerateBase64String(11);
            while (await _condominiumRepository.AnyAsync(x => x.InviteCode == inviteCode, cancellationToken))
            {
                _logger.LogTrace("Failed to create invite code {InviteCode}, already exists.", inviteCode);
                inviteCode = ByteHelpers.GenerateBase64String(11);
            }
            Condominium? createdCondominium = await _condominiumRepository.CreateAsync(new Condominium()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                ImageUrl = imageUploadResult.SecureUrl.ToString(),
                InviteCode = inviteCode,
                LinkToken = "",
                TokenExpirationDate = DateTime.UtcNow.AddDays(90),
            }, cancellationToken);

            if (createdCondominium is null)
            {
                _logger.LogWarning("Failed to create condominium, with request {@request}.}", request);
                return Result<AddCondominiumResponse>.Fail("Failed to create condominium");
            }

            await _condominiumUserRepository.CreateAsync(new()
            {
                UserId = request.userId,
                CondominiumId = createdCondominium.Id,

            }, cancellationToken);
            
            return new AddCondominiumResponse()
            {
                Id = createdCondominium.Id,
                Address = createdCondominium.Address,
                Name = createdCondominium.Name,
                ImageUrl = createdCondominium.ImageUrl
            };
        }
    }
}