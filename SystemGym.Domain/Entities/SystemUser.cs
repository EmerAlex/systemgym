namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.ValueObjects;
using SystemGym.Domain.Exceptions;

/// <summary>
/// Agregado raíz: Usuario del sistema (Admin/Standard)
/// </summary>
public class SystemUser : AggregateRoot
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;
    public Role Role { get; private set; }
    public bool Habilitado { get; private set; } = true;
    public DateTime? LastLogin { get; private set; }

    private SystemUser() { }

    private SystemUser(Guid id, string username, string passwordHash, string passwordSalt, Role role) 
        : base(id)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
        Habilitado = true;
    }

    public static SystemUser Create(string username, string passwordHash, string passwordSalt, string role)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            throw new DomainException("Username must be at least 3 characters");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty");

        var systemUser = new SystemUser(
            Guid.NewGuid(),
            username,
            passwordHash,
            passwordSalt,
            Role.Create(role));

        return systemUser;
    }

    public void Disable()
    {
        if (!Habilitado)
            throw new DomainException("User is already disabled");

        Habilitado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Enable()
    {
        if (Habilitado)
            throw new DomainException("User is already enabled");

        Habilitado = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(string newRole)
    {
        Role = Role.Create(newRole);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
