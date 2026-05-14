namespace SystemGym.Application.DTOs.SystemUsers;

using SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO para crear usuario del sistema
/// </summary>
public class CreateSystemUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Standard";
}

/// <summary>
/// DTO de respuesta para usuario del sistema
/// </summary>
public class SystemUserResponseDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Habilitado { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para actualizar rol de usuario
/// </summary>
public class UpdateSystemUserRoleDto
{
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// DTO para respuesta de listado de usuarios
/// </summary>
public class SystemUsersListResponseDto : PaginatedResponseDto<SystemUserResponseDto>
{
}
