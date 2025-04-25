using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;

namespace CondominiumAlerts.Features.Features.Comment.Update
{
    public class UpdateCommentCommandHandler : ICommandHandler<UpdateCommentCommand, Result<UpdateCommentResponse>>
    {
        private readonly Cloudinary _cloudinary;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<UpdateCommentCommandHandler> _logger;
        private readonly IValidator<UpdateCommentCommand> _validator;

        public UpdateCommentCommandHandler(
            Cloudinary cloudinary,
            ICommentRepository commentRepository,
            ILogger<UpdateCommentCommandHandler> logger,
            IValidator<UpdateCommentCommand> validator) 
        {
            _cloudinary = cloudinary;
            _commentRepository = commentRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result<UpdateCommentResponse>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var validation = _validator.Validate(request); // Fix: This now works because _validator is of type IValidator<UpdateCommentCommand>
            if (!validation.IsValid)
            {
                IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
                _logger.LogTrace($"Validation failed {errors}");
                return Result.Fail<UpdateCommentResponse>(string.Join(", ", errors));
            }

            var existingComment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingComment == null)
            {
                _logger.LogError($"No se encontró el post con ID {request.Id}");
                return Result.Fail<UpdateCommentResponse>("El post no existe");
            }

            existingComment.Text = request.Text;
            existingComment.UpdatedAt = DateTime.UtcNow;

            string imageUrl = existingComment.ImageUrl;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), request.ImageFile.OpenReadStream()),
                });

                if (uploadResult.Error?.Message is { } message)
                {
                    _logger.LogTrace("Failed to upload image, error with message {Error}.", message);
                    return Result.Fail<UpdateCommentResponse>(message);
                }

                imageUrl = uploadResult.SecureUrl.ToString();
                existingComment.ImageUrl = imageUrl;
            }

            // Guardar cambios
            await _commentRepository.UpdateAsync(existingComment, cancellationToken);

            return new UpdateCommentResponse()
            {
                Id = existingComment.Id,
                Text = existingComment.Text,
                ImageUrl = imageUrl
            };
        }
    }
}
