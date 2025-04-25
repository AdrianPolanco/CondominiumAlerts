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
using CondominiumAlerts.Features.Features.Condominiums.Add;
using Microsoft.AspNetCore.SignalR;
using CondominiumAlerts.Features.Features.Notifications;
using Mapster;
using CondominiumAlerts.Features.Features.Notifications.Get;
using CondominiumAlerts.Domain.Interfaces;

public class CreatePostHandler : ICommandHandler<CreatePostCommand, Result<CreatePostResponse>>
{
    private readonly Cloudinary _cloudinary;
    private readonly IPostsRepository _postsRepository;
    private readonly IRepository<User, string> _userRepository;
    private readonly ILogger<CreatePostHandler> _logger;
    private readonly IValidator<CreatePostCommand> _validator;
    private readonly INotificationService _notificationService;
    private readonly IRepository<LevelOfPriority, Guid> _levelOfPriorityRepository;

    public CreatePostHandler(Cloudinary cloudinary,
                            IPostsRepository postsRepository,
                            IRepository<User, string> userRepository,
                            ILogger<CreatePostHandler> logger,
                            IValidator<CreatePostCommand> validator,
                            INotificationService notificationService,
                            IRepository<LevelOfPriority, Guid> levelOfPriorityRepository)
    {
        _cloudinary = cloudinary;
        _postsRepository = postsRepository;
        _userRepository = userRepository;
        _logger = logger;
        _validator = validator;
       _notificationService = notificationService;
        _levelOfPriorityRepository = levelOfPriorityRepository;
    }

    public async Task<Result<CreatePostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Obtener el ID del usuario desde el request
        var userId = request.UserId;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("No se pudo obtener el ID del usuario desde la solicitud.");
            return Result.Fail<CreatePostResponse>("Usuario no autenticado.");
        }

        // Verificar que el usuario existe en la base de datos
        if (!await _userRepository.AnyAsync(u => u.Id == userId, cancellationToken))
        {
            _logger.LogError($"El usuario con ID {userId} no existe en la base de datos.");
            return Result.Fail<CreatePostResponse>("El usuario no existe.");
        }

        // Validar el comando
        var validation = _validator.Validate(request);
        if (!validation.IsValid)
        {
            IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
            _logger.LogTrace($"Validation failed {errors}");
            return Result.Fail<CreatePostResponse>(string.Join(", ", errors));
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
                return Result.Fail<CreatePostResponse>(message);
            }

            imageUrl = imageUploadResult.SecureUrl.ToString();
        }


        _logger.LogInformation($"CondominiumId recibido: {request.CondominiumId}");

        PostsEntity.Post posts = await _postsRepository.CreateAsync(new PostsEntity.Post()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ImageUrl = imageUrl,
            UserId = userId,
            LevelOfPriorityId = request.LevelOfPriorityId,
            CondominiumId = request.CondominiumId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }, cancellationToken);


        var lvlOfPriority = await _levelOfPriorityRepository.GetByIdAsync(request.LevelOfPriorityId, cancellationToken);
       if (lvlOfPriority?.Priority >= 7)
        {
            await _notificationService.Notify(new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Nuevo Post creado",
                Description = $"Nuevo post: {posts.Title}",
                CondominiumId = posts.CondominiumId,
                LevelOfPriorityId = posts.LevelOfPriorityId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, posts.CondominiumId.ToString(), cancellationToken);
        }

        return new CreatePostResponse()
        {
            Id = posts.Id,
            Title = posts.Title,
            Description = posts.Description,
            ImageUrl = posts.ImageUrl,
            CondominiumId = posts.CondominiumId,
            UserId = posts.UserId,
            LevelOfPriorityId = posts.LevelOfPriorityId,
            CreatedAt = posts.CreatedAt,
            UpdatedAt = posts.UpdatedAt
        };
    }
}
