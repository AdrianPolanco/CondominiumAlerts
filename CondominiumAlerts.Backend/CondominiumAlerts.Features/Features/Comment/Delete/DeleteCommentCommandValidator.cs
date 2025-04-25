using FluentValidation;

namespace CondominiumAlerts.Features.Features.Comment.Delete
{
    public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required");
            RuleFor(x => x.PostId)
                .NotEmpty()
                .WithMessage("postId is required");
        }
    }
}
