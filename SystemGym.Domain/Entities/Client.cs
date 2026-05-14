namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.ValueObjects;
using SystemGym.Domain.Exceptions;

/// <summary>
/// Agregado raíz: Cliente del gimnasio
/// </summary>
public class Client : AggregateRoot
{
    public DocumentType TipoDocumento { get; private set; }
    public string NumeroDocumento { get; private set; } = string.Empty;
    public string NombreCompleto { get; private set; } = string.Empty;
    public PhoneNumber Celular { get; private set; }
    public bool Habilitado { get; private set; } = true;

    private readonly List<Subscription> _subscriptions = new();
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

    private Client() { }

    private Client(Guid id, DocumentType tipoDoc, string numeroDoc, string nombre, PhoneNumber celular) 
        : base(id)
    {
        TipoDocumento = tipoDoc;
        NumeroDocumento = numeroDoc;
        NombreCompleto = nombre;
        Celular = celular;
        Habilitado = true;
    }

    public static Client Create(string tipoDocumento, string numeroDocumento, string nombreCompleto, string? celular)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento) || numeroDocumento.Length > 20)
            throw new DomainException("Numero documento invalid");

        if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Length > 150)
            throw new DomainException("Nombre completo invalid");

        var client = new Client(
            Guid.NewGuid(),
            DocumentType.Create(tipoDocumento),
            numeroDocumento,
            nombreCompleto,
            PhoneNumber.Create(celular));

        return client;
    }

    public void Disable()
    {
        if (!Habilitado)
            throw new DomainException("Client is already disabled");

        Habilitado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContactInfo(string nombreCompleto, string? celular)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Length > 150)
            throw new DomainException("Nombre completo invalid");

        NombreCompleto = nombreCompleto;
        Celular = PhoneNumber.Create(celular);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateClient(string tipoDocumento, string numeroDocumento, string nombreCompleto, string? celular, bool habilitado)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento) || numeroDocumento.Length > 20)
            throw new DomainException("Numero documento invalid");

        if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Length > 150)
            throw new DomainException("Nombre completo invalid");

        TipoDocumento = DocumentType.Create(tipoDocumento);
        NumeroDocumento = numeroDocumento;
        NombreCompleto = nombreCompleto;
        Celular = PhoneNumber.Create(celular);
        Habilitado = habilitado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSubscription(Subscription subscription)
    {
        if (_subscriptions.Any(s => s.PlanId == subscription.PlanId))
            throw new DomainException("Subscription already exists for this plan");

        _subscriptions.Add(subscription);
    }
}
