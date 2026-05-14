namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.Auth;

/// <summary>
/// Validador para LoginDto
/// </summary>
public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El usuario es requerido")
            .MinimumLength(4).WithMessage("El usuario debe tener al menos 4 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres");
    }
}
