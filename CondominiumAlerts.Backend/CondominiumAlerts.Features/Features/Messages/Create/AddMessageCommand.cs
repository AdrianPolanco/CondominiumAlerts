using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Messages.Create
{
    public class AddMessageCommand: ICommand<Result<MessageDto>>
    {
        public string Text { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public string? CreatorUserId { get; set; }
        public string? ReceiverUserId { get; set; }
        public Guid? CondominiumId { get; set; }
        public Guid? MessageBeingRepliedToId { get; set; }
    }
}
