namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.Subscriptions;

/// <summary>
/// Validador para CreateSubscriptionDto
/// </summary>
public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El cliente es requerido");

        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("El plan es requerido");

        RuleFor(x => x.InicioVigencia)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("La fecha de inicio debe ser hoy o posterior");
    }
}

/// <summary>
/// Validador para RenewSubscriptionDto
/// </summary>
public class RenewSubscriptionValidator : AbstractValidator<RenewSubscriptionDto>
{
    public RenewSubscriptionValidator()
    {
        RuleFor(x => x.NuevoInicio)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("La nueva fecha de inicio debe ser hoy o posterior");
    }
}
