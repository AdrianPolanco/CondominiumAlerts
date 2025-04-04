using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Posts.Create;
using FluentValidation;
using LightResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using PostsEntity = CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.Entities;


namespace CondominiumAlerts.Features.Features.Posts.Update
{
    public class UpdatePostsHandler : ICommandHandler<UpdatePostsCommand, Result<UpdatePostsResponse>>
    {
        private readonly Cloudinary _cloudinary;
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<UpdatePostsHandler> _logger;
        private readonly IValidator<UpdatePostsCommand> _validator;

        public UpdatePostsHandler(
            Cloudinary cloudinary,
            IPostsRepository postsRepository,
            ILogger<UpdatePostsHandler> logger,
            IValidator<UpdatePostsCommand> validator)
        {
            _cloudinary = cloudinary;
            _postsRepository = postsRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result<UpdatePostsResponse>> Handle(UpdatePostsCommand request, CancellationToken cancellationToken)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
                _logger.LogTrace($"Validation failed {errors}");
                return Result.Fail<UpdatePostsResponse>(string.Join(", ", errors));
            }

            var existingPost = await _postsRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingPost == null)
            {
                _logger.LogError($"No se encontró el post con ID {request.Id}");
                return Result.Fail<UpdatePostsResponse>("El post no existe");
            }

            existingPost.Title = request.Title;
            existingPost.Description = request.Description;
            existingPost.LevelOfPriorityId = request.LevelOfPriorityId;
            existingPost.UpdatedAt = DateTime.UtcNow;

            string imageUrl = existingPost.ImageUrl;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), request.ImageFile.OpenReadStream()),
                });

                if (uploadResult.Error?.Message is { } message)
                {
                    _logger.LogTrace("Failed to upload image, error with message {Error}.", message);
                    return Result.Fail<UpdatePostsResponse>(message);
                }

                imageUrl = uploadResult.SecureUrl.ToString();
                existingPost.ImageUrl = imageUrl;
            }

            // Guardar cambios
            await _postsRepository.UpdateAsync(existingPost, cancellationToken);

            return new UpdatePostsResponse()
            {
                Id = existingPost.Id,
                Title = existingPost.Title,
                Description = existingPost.Description,
                ImageUrl = imageUrl,
                LevelOfPriorityId = existingPost.LevelOfPriorityId
            };
        }
    }
}