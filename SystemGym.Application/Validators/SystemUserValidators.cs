namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.SystemUsers;

/// <summary>
/// Validador para CreateSystemUserDto
/// </summary>
public class CreateSystemUserValidator : AbstractValidator<CreateSystemUserDto>
{
    public CreateSystemUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El usuario es requerido")
            .MinimumLength(4).WithMessage("El usuario debe tener al menos 4 caracteres")
            .MaximumLength(50).WithMessage("El usuario no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-Z0-9_\-\.]+$").WithMessage("El usuario solo puede contener letras, números, punto, guión y guión bajo");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol es requerido")
            .Must(role => role == "Admin" || role == "Standard")
            .WithMessage("El rol debe ser 'Admin' o 'Standard'");
    }
}

/// <summary>
/// Validador para UpdateSystemUserRoleDto
/// </summary>
public class UpdateSystemUserRoleValidator : AbstractValidator<UpdateSystemUserRoleDto>
{
    public UpdateSystemUserRoleValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol es requerido")
            .Must(role => role == "Admin" || role == "Standard")
            .WithMessage("El rol debe ser 'Admin' o 'Standard'");
    }
}
