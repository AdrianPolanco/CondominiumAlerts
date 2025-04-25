using FluentValidation;

namespace CondominiumAlerts.Features.Features.Comment.Update
{
    public class UpdateCommentValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentValidator()
        {
            RuleFor(x => x.Text)
                .MaximumLength(200).WithMessage("El título no puede exceder 100 caracteres");
        }
    }
}
