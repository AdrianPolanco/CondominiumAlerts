using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Interfaces;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Users.Update;
using LightResults;
using Microsoft.AspNetCore.Http;
using Polly;

namespace CondominiumAlerts.Features.Features.Messages.Create
{
    public class AddMessageCommandHandler(IRepository<Message, Guid> messageRepository, Cloudinary cloudinary, IAsyncPolicy retryPolicy) : ICommandHandler<AddMessageCommand, Result<MessageDto>>
    {
       // private readonly INotificationService _notificationService = notificationService;

        public async Task<Result<MessageDto>> Handle(AddMessageCommand request, CancellationToken cancellationToken)
        {
            
            var message = new Message()
            {
                CreatedAt = DateTime.UtcNow,
                CreatorUserId = request.CreatorUserId!,
                CondominiumId = request.CondominiumId,
                Text = request.Text,
                MediaUrl = request.Media
            };

            var messageAdded = await messageRepository.CreateAsync(message, cancellationToken);
            var messageWithCreatorUser = await messageRepository.GetByIdAsync(messageAdded.Id, cancellationToken, includes: [m => m.CreatorUser])!;
            
            UserDto userDto = new(
                messageWithCreatorUser!.CreatorUser.Id,
                messageWithCreatorUser.CreatorUser.Name,
                messageWithCreatorUser.CreatorUser.Lastname,
                messageWithCreatorUser.CreatorUser.ProfilePictureUrl,
                messageWithCreatorUser.CreatorUser.Username);

            return Result.Ok(new MessageDto(
                messageWithCreatorUser!.Id,
                messageWithCreatorUser.Text,
                userDto,
                messageWithCreatorUser.CreatorUserId,
                messageWithCreatorUser.ReceiverUserId,
                messageWithCreatorUser.MediaUrl,
                messageWithCreatorUser.CondominiumId,
                messageWithCreatorUser.MessageBeingRepliedToId,
                messageWithCreatorUser.CreatedAt));
        }
    }
}
