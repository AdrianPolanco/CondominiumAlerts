using CondominiumAlerts.Domain.Aggregates.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Users.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
    
    public UpdateUserValidator()
    {
        RuleFor(u => u.Username).NotEmpty().NotNull().WithMessage("El username es requerido.");         
        RuleFor(u => u.Username).MinimumLength(4).MaximumLength(25).WithMessage("El nombre de usuario debe tener entre 4 y 25 caracteres.");    
        RuleFor(u => u.Name).NotEmpty().WithMessage("El nombre es requerido.");         
        RuleFor(u => u.Lastname).NotEmpty().WithMessage("El apellido es requerido.");         
        RuleFor(u => u.Lastname).MinimumLength(3).MaximumLength(200).WithMessage("El apellido debe tener entre 3 y 200 caracteres.");  
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("El Id es requerido.")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("El Id contiene caracteres no válidos.");
        RuleFor(u => u.ProfilePic)
            .Must(BeAValidImage).WithMessage("El archivo debe ser una imagen válida (JPG, PNG, WEBP).");
        RuleFor(u => u.Address)
            .NotNull().WithMessage("La dirección es obligatoria.")
            .SetValidator(new AddressValidator());
    }
    
    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null) return true;
        return AllowedMimeTypes.Contains(file.ContentType.ToLower());
    }
}