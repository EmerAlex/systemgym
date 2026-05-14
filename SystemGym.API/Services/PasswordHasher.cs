namespace SystemGym.API.Services;

using Isopoh.Cryptography.Argon2;
using SystemGym.Application.Abstractions;

/// <summary>
/// Implementación de hash de contraseñas con Argon2.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return Argon2.Verify(passwordHash, password);
    }
}