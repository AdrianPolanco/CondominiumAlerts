using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Messages.Create
{
    public class AddMessageCommandHandler(IRepository<Message, Guid> messageRepository) : ICommandHandler<AddMessageCommand, Result<MessageDto>>
    {
        public async Task<Result<MessageDto>> Handle(AddMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message()
            {
                CreatedAt = DateTime.UtcNow,
                CondominiumId = request.CondominiumId,
                Text = request.Text,
                MediaUrl = request.MediaUrl,
            };

            var messageAdded = await messageRepository.CreateAsync(message, cancellationToken);
            return Result.Ok(new MessageDto(
                messageAdded.Id,
                messageAdded.Text,  
                messageAdded.MediaUrl,  
                messageAdded.CondominiumId, 
                messageAdded.ReceiverUserId,
                messageAdded.MessageBeingRepliedToId, message.CreatedAt));
        }
    }
}
