namespace SystemGym.Application.Features.Plans.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Handler para el comando de eliminar plan
/// </summary>
public class DeletePlanCommandHandler : ICommandHandler<DeletePlanCommand, DeletePlanCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePlanCommandHandler> _logger;

    public DeletePlanCommandHandler(IUnitOfWork unitOfWork, ILogger<DeletePlanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeletePlanCommandResult> Handle(
        DeletePlanCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(request.PlanId, cancellationToken);
            if (plan is null)
                return new DeletePlanCommandResult { Success = false, Message = "Plan no encontrado" };

            _unitOfWork.Plans.Delete(plan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Plan eliminado: {PlanId}", request.PlanId);

            return new DeletePlanCommandResult
            {
                Success = true,
                Message = "Plan eliminado exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar plan {PlanId}", request.PlanId);
            return new DeletePlanCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
