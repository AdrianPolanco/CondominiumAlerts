

using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominiums.Join
{
    public class JoinCondominiumValidator : AbstractValidator<JoinCondominiumCommand>
    {
        public JoinCondominiumValidator() { 
            RuleFor(jc => jc.UserId).NotEmpty().WithMessage("The user was not especified");

            RuleFor(jc => jc.CondominiumCode).NotEmpty().WithMessage("The code was not passed");
            RuleFor(jc => jc.CondominiumCode).MinimumLength(10).WithMessage("The code is too short, it must be 11 characters long");
            RuleFor(jc => jc.CondominiumCode).MaximumLength(12).WithMessage("The code is too long, it must be 11 characters long");
        }
    }
}
