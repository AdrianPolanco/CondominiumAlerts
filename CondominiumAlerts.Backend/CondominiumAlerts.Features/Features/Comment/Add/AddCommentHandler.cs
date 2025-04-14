using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;
using CommentsEntity = CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Comment.Add
{
    public class AddCommentHandler : ICommandHandler<AddCommentCommand, Result<AddCommentResponse>>
    {
        private readonly Cloudinary _cloudinary;
        private readonly ICommentRepository _commentRepository;
        private readonly IRepository<User, string> _userRepository;
        private readonly ILogger<AddCommentHandler> _logger;
        private readonly IValidator<AddCommentCommand> _validator;

        public AddCommentHandler(Cloudinary cloudinary,
                                ICommentRepository commentRepository,
                                IRepository<User, string> userRepository,
                                ILogger<AddCommentHandler> logger,
                                IValidator<AddCommentCommand> validator)
        {
            _cloudinary = cloudinary;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result<AddCommentResponse>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            // Obtener el ID del usuario desde el request
            var userId = request.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("No se pudo obtener el ID del usuario desde la solicitud.");
                return Result.Fail<AddCommentResponse>("Usuario no autenticado.");
            }

            // Verificar que el usuario existe en la base de datos
            if (!await _userRepository.AnyAsync(u => u.Id == userId, cancellationToken))
            {
                _logger.LogError($"El usuario con ID {userId} no existe en la base de datos.");
                return Result.Fail<AddCommentResponse>("El usuario no existe.");
            }

            // Validar el comando
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
                _logger.LogTrace($"Validation failed {errors}");
                return Result.Fail<AddCommentResponse>(string.Join(", ", errors));
            }

            string? imageUrl = null;

            if (request.ImageFile != null)
            {
                ImageUploadResult imageUploadResult = await _cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), request.ImageFile.OpenReadStream()),
                });
                if (imageUploadResult.Error?.Message is { } message)
                {
                    _logger.LogTrace("Failed to upload image, error with message {Error}.", message);
                    return Result.Fail<AddCommentResponse>(message);
                }

                imageUrl = imageUploadResult.SecureUrl.ToString();
            }

            CommentsEntity.Comment comment = await _commentRepository.CreateAsync(new CommentsEntity.Comment()
            {
                Id = Guid.NewGuid(),
                Text = request.Text,
                ImageUrl = imageUrl,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            return new AddCommentResponse()
            {
                Id = comment.Id,
                Text = comment.Text,
                ImageUrl = comment.ImageUrl,
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}
