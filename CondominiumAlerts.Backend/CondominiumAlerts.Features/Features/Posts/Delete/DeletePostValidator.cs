using FluentValidation;

namespace CondominiumAlerts.Features.Features.Posts.Delete
{
    public class DeletePostValidator : AbstractValidator<DeletePostCommand>
    {
        public DeletePostValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required");
            RuleFor(x => x.CondominiumId)
                .NotEmpty()
                .WithMessage("CondominiumId is required");
        }
    }
}
