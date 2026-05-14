namespace SystemGym.Domain.Exceptions;

/// <summary>
/// Excepción base de dominio
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    
    public DomainException(string message, Exception innerException) 
        : base(message, innerException) { }
}
