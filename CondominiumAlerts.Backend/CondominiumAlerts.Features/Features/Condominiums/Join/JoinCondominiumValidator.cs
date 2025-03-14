

using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominiums.Join
{
    public class JoinCondominiumValidator : AbstractValidator<JoinCondominiumCommand>
    {
        public JoinCondominiumValidator() { 
            RuleFor(jc => jc.UserId).NotEmpty().WithMessage("The user was not especified");

            RuleFor(jc => jc.CondominiumCode).NotEmpty().WithMessage("The code was not passed");

            RuleFor(jc => jc.CondominiumCode).MinimumLength(11).WithMessage("The code is to short, it must be 11 characters long");
            RuleFor(jc => jc.CondominiumCode).MaximumLength(11).WithMessage("The code is to long, it must be 11 characters long");
        }
    }
}
