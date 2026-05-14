namespace SystemGym.Application.Features.Plans.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Handler para el comando de actualizar plan
/// </summary>
public class UpdatePlanCommandHandler : ICommandHandler<UpdatePlanCommand, UpdatePlanCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePlanCommandHandler> _logger;

    public UpdatePlanCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePlanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdatePlanCommandResult> Handle(
        UpdatePlanCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(request.PlanId, cancellationToken);
            if (plan is null)
                return new UpdatePlanCommandResult { Success = false, Message = "Plan no encontrado" };

            plan.UpdateInfo(
                request.Descripcion,
                request.TipoPeriodo,
                request.CantidadIntervalosPeriodo,
                request.Valor);

            _unitOfWork.Plans.Update(plan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Plan actualizado: {PlanId}", plan.Id);

            return new UpdatePlanCommandResult
            {
                Success = true,
                Message = "Plan actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plan {PlanId}", request.PlanId);
            return new UpdatePlanCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
