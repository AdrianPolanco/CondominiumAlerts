using CloudinaryDotNet;
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Interfaces;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Messages.Create
{
    public class AddMessageCommandHandler(IRepository<Message, Guid> messageRepository, Cloudinary cloudinary/*, INotificationService notificationService*/) : ICommandHandler<AddMessageCommand, Result<MessageDto>>
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
            };

            var messageAdded = await messageRepository.CreateAsync(message, cancellationToken);
            var messageWithCreatorUser = await messageRepository.GetByIdAsync(messageAdded.Id, cancellationToken, includes: [m => m.CreatorUser])!;
            UserDto userDto = new(
                messageWithCreatorUser!.CreatorUser.Id,
                messageWithCreatorUser.CreatorUser.Name,
                messageWithCreatorUser.CreatorUser.Lastname,
                messageWithCreatorUser.CreatorUser.ProfilePictureUrl,
                messageWithCreatorUser.CreatorUser.Username);
            if (message.CondominiumId.HasValue)
            {
                await _notificationService.Notify(new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Nuevo mensaje",
                    Description = $"Nuevo mensaje de {messageWithCreatorUser.CreatorUser?.Name}",
                    CondominiumId = message.CondominiumId.Value,
                    CreatedAt = DateTime.UtcNow,
                    ReceiverUserId = request.ReceiverUserId // Only notify specific user if it's a direct message
                }, message.CondominiumId.Value.ToString(), cancellationToken);
            }

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
