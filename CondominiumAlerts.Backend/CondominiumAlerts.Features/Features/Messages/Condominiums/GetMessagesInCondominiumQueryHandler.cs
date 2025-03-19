using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Messages.Condominiums;

public class GetMessagesInCondominiumQueryHandler : IQueryHandler<GetMessagesInCondominiumQuery, Result<List<ChatMessageDto>>>
{
    private readonly IRepository<Message, Guid> _messagesRepository;

    public GetMessagesInCondominiumQueryHandler(IRepository<Message, Guid> messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }
    
    public async Task<Result<List<ChatMessageDto>>> Handle(GetMessagesInCondominiumQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messagesRepository.GetAsync(
            cancellationToken: cancellationToken,
            filter: m => m.CondominiumId == request.CondominiumId,
            includes: [m => m.CreatorUser]
        );

        if (!messages.Any()) return Result.Fail<List<ChatMessageDto>>("No se encontraron mensajes para el condominio dado.");

       /* var result = messages.Select(m =>
        {
            var creatorUser = new ChatCreatorUserDto(
                Id: m.CreatorUser.Id,
                Name: m.CreatorUser.Name,
                Lastname: m.CreatorUser.Lastname,
                ProfilePictureUrl: m.CreatorUser.ProfilePictureUrl,
                Username: m.CreatorUser.Username
                );
            
            var creatorUser = m.CreatorUser.Adapt<ChatCreatorUserDto>();

            var chatMessageDto = new ChatMessageDto(
                Id: m.Id, 
                Text: m.Text, 
                CondominiumId: m.CondominiumId, 
                ReceiverUserId: m.ReceiverUserId,
                MessageBeingRepliedToId: m.MessageBeingRepliedToId,
                CreatedAt: m.CreatedAt,
                CreatorUser: creatorUser,
                MediaUrl: m.MediaUrl);
            
            return chatMessageDto;
        }).ToList();*/
        
        var result = messages.Adapt<List<ChatMessageDto>>();
        result = result.OrderBy(r => r.CreatedAt).ToList();
        
        return Result.Ok(result);
    }
}