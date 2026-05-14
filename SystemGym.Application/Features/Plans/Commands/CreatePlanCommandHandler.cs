namespace SystemGym.Application.Features.Plans.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para el comando de crear plan
/// </summary>
public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, CreatePlanCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePlanCommandHandler> _logger;

    public CreatePlanCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePlanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreatePlanCommandResult> Handle(
        CreatePlanCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = Plan.Create(
                request.Descripcion,
                request.TipoPeriodo,
                request.CantidadIntervalosPeriodo,
                request.Valor);

            _unitOfWork.Plans.Add(plan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Plan creado: {PlanId}", plan.Id);

            return new CreatePlanCommandResult
            {
                Success = true,
                Message = "Plan creado exitosamente",
                Data = plan.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plan");
            return new CreatePlanCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
