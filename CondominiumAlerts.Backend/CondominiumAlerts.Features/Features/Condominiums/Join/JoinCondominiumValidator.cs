using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominiums.Join
{
    public class JoinCondominiumValidator : AbstractValidator<JoinCondominiumCommand>
    {
        public JoinCondominiumValidator()
        {
            RuleFor(jc => jc.UserId)
                .NotEmpty().WithMessage("The user was not specified");

            RuleFor(jc => jc.CondominiumCode)
                .NotEmpty()
                .When(jc => jc.CondominiumToken is null or "").WithMessage("The code was not passed");

            RuleFor(jc => jc.CondominiumCode)
                .MinimumLength(11)
                .When(jc => jc.CondominiumToken is null or "")
                .WithMessage("The code is to short, it must be 11 characters long");
            
            RuleFor(jc => jc.CondominiumCode)
                .MaximumLength(11)
                .When(jc => jc.CondominiumToken is null or "")
                .WithMessage("The code is to long, it must be 11 characters long");
            
            RuleFor(jc => jc.CondominiumToken)
                .NotEmpty()
                .When(t => t.CondominiumCode is null)
                .WithMessage("The token was not provided");
        }
    }
}