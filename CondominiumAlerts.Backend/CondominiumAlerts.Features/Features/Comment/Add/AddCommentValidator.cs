using FluentValidation;


namespace CondominiumAlerts.Features.Features.Comment.Add
{
   public class AddCommentValidator : AbstractValidator<AddCommentCommand>
   {
        public AddCommentValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID de usuario es requerido");


            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Text) || x.ImageFile != null)
                .WithMessage("El comentario debe contener texto o una imagen");
        }

   }
}
