namespace SystemGym.Application.Features.Subscriptions.Queries;

using System.Text;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Utilidades compartidas de generacion CSV para suscripciones.
/// Centraliza el formato y escape RFC 4180 para evitar duplicacion entre handlers.
/// </summary>
internal static class SubscriptionCsvHelper
{
    internal const string CsvHeader =
        "SubscriptionId,TipoDocumento,NumeroDocumento,NombreCliente,Plan," +
        "InicioVigencia,FinVigencia,Activa,TieneExpiracion,Valor,CantidadIngresos,UltimoIngreso";

    internal static void AppendRow(StringBuilder sb, Subscription s, Client c, string planDesc)
        => sb.AppendLine(string.Join(",",
            Csv(s.Id.ToString()),
            Csv(c.TipoDocumento.Value),
            Csv(c.NumeroDocumento),
            Csv(c.NombreCompleto),
            Csv(planDesc),
            Csv(s.InicioVigencia.ToString("yyyy-MM-dd")),
            Csv(s.FinVigencia?.ToString("yyyy-MM-dd") ?? ""),
            Csv(s.Activa ? "Si" : "No"),
            Csv(s.TieneExpiracion ? "Si" : "No"),
            Csv(s.Valor.ToString("F2")),
            Csv(s.CantidadIngresos.ToString()),
            Csv(s.UltimoIngreso?.ToString("yyyy-MM-dd") ?? "")));

    internal static string Csv(string value)
        => value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;

    internal static byte[] ToUtf8WithBom(StringBuilder sb)
        => Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
}

/// <summary>
/// Handler para exportar suscripciones de un cliente especifico como CSV
/// </summary>
public class ExportClientSubscriptionsCsvQueryHandler
    : IQueryHandler<ExportClientSubscriptionsCsvQuery, byte[]>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportClientSubscriptionsCsvQueryHandler> _logger;

    public ExportClientSubscriptionsCsvQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<ExportClientSubscriptionsCsvQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<byte[]> Handle(
        ExportClientSubscriptionsCsvQuery request,
        CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente {request.ClientId} no encontrado");

        var subs    = await _unitOfWork.Subscriptions.GetAllByClientIdAsync(request.ClientId, cancellationToken);
        var planMap = await BuildPlanMapAsync(subs, cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine(SubscriptionCsvHelper.CsvHeader);
        foreach (var s in subs)
            SubscriptionCsvHelper.AppendRow(sb, s, client, planMap.GetValueOrDefault(s.PlanId, ""));

        return SubscriptionCsvHelper.ToUtf8WithBom(sb);
    }

    private async Task<Dictionary<Guid, string>> BuildPlanMapAsync(
        IEnumerable<Subscription> subs,
        CancellationToken ct)
    {
        var map = new Dictionary<Guid, string>();
        foreach (var planId in subs.Select(s => s.PlanId).Distinct())
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(planId, ct);
            if (plan is not null) map[planId] = plan.Descripcion;
        }
        return map;
    }
}

/// <summary>
/// Handler para exportar resultado de busqueda global como CSV
/// </summary>
public class ExportSubscriptionsCsvQueryHandler
    : IQueryHandler<ExportSubscriptionsCsvQuery, byte[]>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportSubscriptionsCsvQueryHandler> _logger;

    public ExportSubscriptionsCsvQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<ExportSubscriptionsCsvQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<byte[]> Handle(
        ExportSubscriptionsCsvQuery request,
        CancellationToken cancellationToken)
    {
        List<Subscription> subs;

        if (!string.IsNullOrWhiteSpace(request.NumeroDocumento))
        {
            var c = await _unitOfWork.Clients.GetByNumeroDocumentoAsync(
                request.NumeroDocumento.Trim(),
                request.TipoDocumento,
                cancellationToken);

            subs = c is null ? [] : (await _unitOfWork.Subscriptions.GetAllByClientIdAsync(c.Id, cancellationToken)).ToList();
        }
        else if (!string.IsNullOrWhiteSpace(request.NombreCliente))
        {
            var clientes = (await _unitOfWork.Clients.SearchByNombreAsync(
                request.NombreCliente.Trim(), cancellationToken)).ToList();

            subs = [];
            foreach (var client in clientes)
            {
                var clientSubs = await _unitOfWork.Subscriptions.GetAllByClientIdAsync(client.Id, cancellationToken);
                subs.AddRange(clientSubs);
            }
        }
        else
        {
            // Sin parámetros: exportar TODAS las suscripciones
            subs = (await _unitOfWork.Subscriptions.GetAllAsync(cancellationToken)).ToList();
        }

        // Evaluar y auto-desactivar
        var anyChanged = false;
        foreach (var sub in subs)
        {
            if (sub.EvaluateAndDeactivateIfNeeded())
            {
                _unitOfWork.Subscriptions.Update(sub);
                anyChanged = true;
            }
        }
        if (anyChanged)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine(SubscriptionCsvHelper.CsvHeader);

        foreach (var s in subs)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(s.ClientId, cancellationToken);
            var plan = await _unitOfWork.Plans.GetByIdAsync(s.PlanId, cancellationToken);
            
            if (client is not null)
                SubscriptionCsvHelper.AppendRow(sb, s, client, plan?.Descripcion ?? "");
        }

        return SubscriptionCsvHelper.ToUtf8WithBom(sb);
    }
}
