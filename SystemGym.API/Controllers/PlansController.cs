namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.Features.Plans.Commands;
using SystemGym.Application.Features.Plans.Queries;
using SystemGym.Application.DTOs.Plans;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Controlador para gestionar planes de suscripción
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PlansController : BaseController
{
    public PlansController(ILogger<PlansController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Crear nuevo plan
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan(
        [FromBody] CreatePlanDto createPlanDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreatePlanCommand
            {
                Descripcion = createPlanDto.Descripcion,
                TipoPeriodo = createPlanDto.TipoPeriodo,
                CantidadIntervalosPeriodo = createPlanDto.CantidadIntervalosPeriodo,
                Valor = createPlanDto.Valor
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Plan creado: {PlanId}", result.Data);

            return CreatedResult(result.Data, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al crear plan");
            return InternalServerErrorResult("Error al crear el plan");
        }
    }

    /// <summary>
    /// Obtener todos los planes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetPlansQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Planes obtenidos: {Count} items", result.Data.Count);

            return OkResult(result, "Planes obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener planes");
            return InternalServerErrorResult("Error al obtener los planes");
        }
    }

    /// <summary>
    /// Obtener plan por ID
    /// </summary>
    [HttpGet("{planId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlan(
        [FromRoute] Guid planId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetPlanQuery { PlanId = planId };

            var result = await Mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFoundResult("Plan no encontrado");

            return OkResult(result, "Plan obtenido exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener plan {PlanId}", planId);
            return InternalServerErrorResult("Error al obtener el plan");
        }
    }

    /// <summary>
    /// Actualizar plan (solo Admin)
    /// </summary>
    [HttpPut("{planId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlan(
        [FromRoute] Guid planId,
        [FromBody] UpdatePlanDto updatePlanDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdatePlanCommand
            {
                PlanId = planId,
                Descripcion = updatePlanDto.Descripcion,
                TipoPeriodo = updatePlanDto.TipoPeriodo,
                CantidadIntervalosPeriodo = updatePlanDto.CantidadIntervalosPeriodo,
                Valor = updatePlanDto.Valor
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Plan actualizado: {PlanId}", planId);

            return OkResult(null, result.Message ?? "Plan actualizado exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al actualizar plan {PlanId}", planId);
            return InternalServerErrorResult("Error al actualizar el plan");
        }
    }

    /// <summary>
    /// Eliminar plan (solo Admin)
    /// </summary>
    [HttpDelete("{planId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlan(
        [FromRoute] Guid planId,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeletePlanCommand { PlanId = planId };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, null);

            Logger.LogInformation("Plan eliminado: {PlanId}", planId);

            return OkResult(null, result.Message ?? "Plan eliminado exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al eliminar plan {PlanId}", planId);
            return InternalServerErrorResult("Error al eliminar el plan");
        }
    }
}
