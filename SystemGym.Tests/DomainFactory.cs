namespace SystemGym.Tests;

using SystemGym.Domain.Entities;

/// <summary>
/// Factory helper para crear entidades de dominio en tests.
/// Usa los factory methods público del dominio (Client.Create, etc).
/// </summary>
internal static class DomainFactory
{
    public static Client CreateClient(
        string tipoDocumento = "CC",
        string numeroDocumento = "12345678",
        string nombreCompleto = "Juan Pérez",
        string? celular = null)
    {
        return Client.Create(tipoDocumento, numeroDocumento, nombreCompleto, celular);
    }

    public static Plan CreatePlan(
        string descripcion = "Plan Básico",
        decimal valor = 50000,
        string tipoPeriodo = "Mes",
        int cantidadIntervalosPeriodo = 1)
    {
        return Plan.Create(descripcion, tipoPeriodo, cantidadIntervalosPeriodo, valor);
    }

    public static Subscription CreateSubscription(
        Client client,
        Plan plan,
        DateTime? inicio = null)
    {
        inicio ??= DateTime.Today;
        var fin = inicio.Value.AddMonths(1);
        return Subscription.Create(
            client.Id,
            plan.Id,
            inicio.Value,
            tieneExpiracion: true,
            fin,
            plan.Valor,
            Subscription.CalculateIngresos(plan.TipoPeriodo.Value, plan.CantidadIntervalosPeriodo));
    }
}
