using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Helpers;
using LightResults;
using Microsoft.Extensions.Logging;
using CondominiumEntity = CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Condominium.Add
{
    public class AddCondominiumHandler
    : ICommandHandler<AddCondominiumCommand, Result<AddCondominiumResponse>>
    {
        private readonly Cloudinary _cloudinary;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly ILogger<AddCondominiumHandler> _logger;

        public AddCondominiumHandler(Cloudinary cloudinary, ICondominiumRepository condominiumRepository, ILogger<AddCondominiumHandler> logger)
        {
            _cloudinary = cloudinary;
            _condominiumRepository = condominiumRepository;
            _logger = logger;
        }

        public async Task<Result<AddCondominiumResponse>>
        Handle(AddCondominiumCommand request, CancellationToken cancellationToken)
        {
            ImageUploadResult imageUploadResult = await _cloudinary.UploadAsync(new ImageUploadParams()
            {
                File = new FileDescription(Guid.NewGuid().ToString(), request.ImageFile.OpenReadStream()),
            });
            if (imageUploadResult.Error?.Message is { } message) {
                _logger.LogTrace("Failed to upload image, error with message {Error}.", message);
                return Result.Fail(message);
            }

            string inviteCode = ByteHelpers.GenerateBase64String(7);
            while (await _condominiumRepository.AnyAsync(x => x.InviteCode == inviteCode, cancellationToken))
            {
                _logger.LogTrace("Failed to create invite code {InviteCode}, already exists.", inviteCode);
                inviteCode = ByteHelpers.GenerateBase64String(7);
            }
            CondominiumEntity.Condominium condominium = await _condominiumRepository.CreateAsync(new CondominiumEntity.Condominium()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                ImageUrl = imageUploadResult.SecureUrl.ToString(),
                InviteCode = inviteCode,
                LinkToken = "",
                TokenExpirationDate = DateTime.UtcNow.AddDays(90),
            }, cancellationToken);
            return new AddCondominiumResponse()
            {
                Id = condominium.Id,
                Address = condominium.Address,
                Name = condominium.Name,
                ImageUrl = condominium.ImageUrl
            };
        }
    }
}