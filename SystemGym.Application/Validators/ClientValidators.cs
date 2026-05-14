namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.Clients;

/// <summary>
/// Validador para CreateClientDto
/// </summary>
public class CreateClientValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.TipoDocumento).TipoDocumentoRules();
        RuleFor(x => x.NumeroDocumento).NumeroDocumentoRules();
        RuleFor(x => x.NombreCompleto).NombreCompletoRules();
        RuleFor(x => x.Celular).CelularOpcionalRules()
            .When(x => !string.IsNullOrWhiteSpace(x.Celular));
    }
}

/// <summary>
/// Validador para UpdateClientDto
/// </summary>
public class UpdateClientValidator : AbstractValidator<UpdateClientDto>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x.NombreCompleto).NombreCompletoRules();
        RuleFor(x => x.Celular).CelularOpcionalRules()
            .When(x => !string.IsNullOrWhiteSpace(x.Celular));
    }
}
