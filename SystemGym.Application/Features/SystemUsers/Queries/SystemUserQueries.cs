namespace SystemGym.Application.Features.SystemUsers.Queries;

using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.SystemUsers;

/// <summary>
/// Query para obtener usuarios del sistema paginados
/// </summary>
public class GetSystemUsersQuery : IQuery<GetSystemUsersQueryResult>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

/// <summary>
/// Resultado de GetSystemUsersQuery
/// </summary>
public class GetSystemUsersQueryResult
{
    public List<SystemUserResponseDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Query para obtener un usuario específico
/// </summary>
public class GetSystemUserQuery : IQuery<SystemUserResponseDto?>
{
    public Guid UserId { get; set; }
}
