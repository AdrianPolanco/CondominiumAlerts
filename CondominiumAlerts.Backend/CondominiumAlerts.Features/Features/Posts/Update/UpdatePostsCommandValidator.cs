using FluentValidation;
using CondominiumAlerts.Features.Features.Posts.Update;

namespace CondominiumAlerts.Features.Validators.Posts
{
    public class UpdatePostsCommandValidator : AbstractValidator<UpdatePostsCommand>
    {
        public UpdatePostsCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El t�tulo es requerido")
                .MaximumLength(100).WithMessage("El t�tulo no puede exceder 100 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripci�n es requerida")
                .MaximumLength(500).WithMessage("La descripci�n no puede exceder 500 caracteres");

            RuleFor(x => x.LevelOfPriorityId)
                .NotEmpty().WithMessage("El nivel de prioridad es requerido");
        }
    }
}