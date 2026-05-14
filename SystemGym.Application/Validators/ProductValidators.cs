namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.Products;

/// <summary>
/// Validador para CreateProductDto
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Descripcion).DescripcionRules();
        RuleFor(x => x.Valor).ValorPositivoRules();
    }
}

/// <summary>
/// Validador para UpdateProductDto
/// </summary>
public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Descripcion).DescripcionRules();
        RuleFor(x => x.Valor).ValorPositivoRules();
    }
}
