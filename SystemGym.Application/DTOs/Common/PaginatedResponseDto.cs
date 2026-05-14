namespace SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO genérico para respuestas paginadas.
/// Reemplaza los DTOs específicos como ClientsListResponseDto, PlansListResponseDto, etc.
/// </summary>
public class PaginatedResponseDto<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public static PaginatedResponseDto<T> Create(
        List<T> data,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        return new PaginatedResponseDto<T>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
