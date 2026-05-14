namespace SystemGym.Application.DTOs.Auth;

/// <summary>
/// DTO para login
/// </summary>
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO de respuesta para login
/// </summary>
public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string? Role { get; set; }
    public string? Message { get; set; }
}
