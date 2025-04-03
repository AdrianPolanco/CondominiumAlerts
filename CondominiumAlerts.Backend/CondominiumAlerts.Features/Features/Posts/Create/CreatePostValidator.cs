using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Posts.Create
{
    public class CreatePostValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostValidator()
        {

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("El título es requerido.");

            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("La descripción es requerida.");

            RuleFor(c => c.LevelOfPriorityId)
                .NotEmpty().WithMessage("El nivel de prioridad es requerido.");
        }
    }
}