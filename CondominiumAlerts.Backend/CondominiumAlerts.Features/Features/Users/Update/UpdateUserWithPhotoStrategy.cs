using System.Net;
using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;
using Microsoft.Extensions.Logging;
using Polly;

namespace CondominiumAlerts.Features.Features.Users.Update;

public class UpdateUserWithPhotoStrategy : IUpdateUserStrategy
{
    private readonly IRepository<User, string> _repository;
    private readonly IUpdateUserStrategy _innerStrategy;
    private readonly Cloudinary _cloudinary;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<UpdateUserWithPhotoStrategy> _logger;

    public UpdateUserWithPhotoStrategy(
        IRepository<User, string> repository, 
        IUpdateUserStrategy innerStrategy, 
        Cloudinary cloudinary,
        IAsyncPolicy retryPolicy,
        ILogger<UpdateUserWithPhotoStrategy> logger)
    {
        _repository = repository;
        _innerStrategy = innerStrategy;
        _cloudinary = cloudinary;
        _retryPolicy = retryPolicy;
        _logger = logger;
    }
    public bool CanHandle(UpdateUserCommand input)
    {
        var canHandle = input.ProfilePic is not null;
        _logger.LogInformation($"UpdateUserWithPhotoStrategy.CanHandle: {canHandle}");
        return canHandle;
    }

    public async Task<Result<UpdateUserResponse>> HandleAsync(UpdateUserCommand input, CancellationToken cancellationToken)
    {
        
        var result = await _innerStrategy.HandleAsync(input, cancellationToken);
        
        if(!result.IsSuccess) return result;
        
        if(!CanHandle(input) && _innerStrategy.CanHandle(input)) return result;
        
        var oldProfilePictureUrl = result.Value.ProfilePictureUrl;

        if (!string.IsNullOrEmpty(oldProfilePictureUrl))
        {
            var oldProfilePictureId = ExtractImageIdFromUrl(oldProfilePictureUrl);
            var deleteParams = new DeletionParams(oldProfilePictureId);
            var deleteResult =  await _retryPolicy.ExecuteAsync(async () => await _cloudinary.DestroyAsync(deleteParams));
                    
            if (deleteResult.StatusCode != HttpStatusCode.OK) return Result<UpdateUserResponse>.Fail($"No se pudo eliminar la imagen anterior. Id {oldProfilePictureId}. Código: {deleteResult.StatusCode}. Error: {deleteResult.Error}. Url: {oldProfilePictureUrl}");
        }


        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(input.ProfilePic.FileName, input.ProfilePic.OpenReadStream()),
        };
        
        var uploadResult = await _retryPolicy.ExecuteAsync(async () => await _cloudinary.UploadAsync(uploadParams));

        if (uploadResult.StatusCode != HttpStatusCode.OK)
            return Result<UpdateUserResponse>.Fail($"No se pudo subir la nueva foto de perfil.");

        var user = await _retryPolicy.ExecuteAsync(async () => await _repository.GetByIdAsync(result.Value.Id, cancellationToken));
        
        user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
        
        user = await _retryPolicy.ExecuteAsync(async () => await _repository.UpdateAsync(user, cancellationToken));
        
        var response = user.Adapt<UpdateUserResponse>();
        
        return response;
    }
    
    // Método para extraer el ID de la imagen de la URL de Cloudinary
    private string ExtractImageIdFromUrl(string url)
    {
        // La URL de Cloudinary tiene este formato: https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{public_id}.jpg
        var regex = new Regex(@"(?<=/v\d+/)([^/]+)", RegexOptions.IgnoreCase); // Busca el ID de la imagen
        var match = regex.Match(url);
        
        return match.Success ? match.Value : string.Empty; // Retorna el ID o cadena vacía si no se encuentra
    }
}