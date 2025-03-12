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
using CondominiumAlerts.Features.Features.Condominium.Add;

public class CreatePostHandler : ICommandHandler<CreatePostCommand, Result<CreatePostResponse>>
{
    private readonly Cloudinary _cloudinary;
    private readonly IPostsRepository _postsRepository;
    private readonly ILogger<CreatePostHandler> _logger;
    private readonly IValidator<CreatePostCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor; 

    public CreatePostHandler(Cloudinary cloudinary,
                            IPostsRepository postsRepository,
                            ILogger<CreatePostHandler> logger,
                            IValidator<CreatePostCommand> validator,
                            IHttpContextAccessor httpContextAccessor) 
    {
        _cloudinary = cloudinary;
        _postsRepository = postsRepository;
        _logger = logger;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor; 
    }

    public async Task<Result<CreatePostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Obtener el ID del usuario logueado
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("No se pudo obtener el ID del usuario logueado.");
            return Result.Fail<CreatePostResponse>("Usuario no autenticado.");
        }

        // Validar el comando
        var validation = _validator.Validate(request);
        if (!validation.IsValid)
        {
            IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
            _logger.LogTrace($"Validation failed {errors}");
            return Result.Fail<CreatePostResponse>(string.Join(", ", errors));
        }

        // Subir la imagen a Cloudinary
        var imageUploadResult = await _cloudinary.UploadAsync(new ImageUploadParams()
        {
            File = new FileDescription(Guid.NewGuid().ToString(), request.ImageFile.OpenReadStream()),
        });

        if (imageUploadResult.Error?.Message is { } message)
        {
            _logger.LogTrace("Failed to upload image, error with message {Error}.", message);
            return Result.Fail<CreatePostResponse>(message);
        }

        PostsEntity.Post posts = await _postsRepository.CreateAsync(new PostsEntity.Post()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ImageUrl = imageUploadResult.SecureUrl.ToString(),
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow, 
            UpdatedAt = DateTime.UtcNow 
        }, cancellationToken);
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