namespace SystemGym.Application.Features.SystemUsers.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.SystemUsers;

/// <summary>
/// Handler para GetSystemUsersQuery
/// </summary>
public class GetSystemUsersQueryHandler : IQueryHandler<GetSystemUsersQuery, GetSystemUsersQueryResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSystemUsersQueryHandler> _logger;

    public GetSystemUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSystemUsersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSystemUsersQueryResult> Handle(GetSystemUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuarios: página {PageNumber}, tamaño {PageSize}", request.PageNumber, request.PageSize);

            var (items, total) = await _unitOfWork.SystemUsers
                .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var users = items.AsEnumerable();

            // Filtrar por nombre de usuario si se proporciona
            if (!string.IsNullOrEmpty(request.SearchTerm))
                users = users.Where(u => u.Username.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));

            var usersList = users.ToList();
            var userDtos = _mapper.Map<List<SystemUserResponseDto>>(usersList);

            return new GetSystemUsersQueryResult
            {
                Data = userDtos,
                TotalCount = usersList.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(usersList.Count / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            throw;
        }
    }
}

/// <summary>
/// Handler para GetSystemUserQuery
/// </summary>
public class GetSystemUserQueryHandler : IQueryHandler<GetSystemUserQuery, SystemUserResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSystemUserQueryHandler> _logger;

    public GetSystemUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSystemUserQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SystemUserResponseDto?> Handle(GetSystemUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuario: {UserId}", request.UserId);

            var user = await _unitOfWork.SystemUsers.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Usuario no encontrado: {UserId}", request.UserId);
                return null;
            }

            return _mapper.Map<SystemUserResponseDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId}", request.UserId);
            throw;
        }
    }
}
