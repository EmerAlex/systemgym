namespace SystemGym.API.Controllers;

using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Auth;
using SystemGym.Application.DTOs.SystemUsers;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Controlador para autenticación de usuarios
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(
        ILogger<AuthController> logger,
        IMediator mediator,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
        : base(logger, mediator)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Registrar nuevo usuario del sistema
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] CreateSystemUserDto createUserDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new SystemGym.Application.Features.SystemUsers.Commands.CreateSystemUserCommand
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password,
                Role = createUserDto.Role
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Usuario registrado: {Username}", createUserDto.Username);

            return CreatedResult(
                result.Data,
                "Usuario registrado exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al registrar usuario");
            return InternalServerErrorResult("Error al registrar el usuario");
        }
    }

    /// <summary>
    /// Login con credenciales
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto loginDto,
        CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Intento de login: {Username}", loginDto.Username);

            var user = await _unitOfWork.SystemUsers.FirstOrDefaultAsync(
                systemUser => systemUser.Username.ToLower() == loginDto.Username.ToLower(),
                cancellationToken);

            if (user is null || !user.Habilitado || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                Logger.LogWarning("Login inválido para usuario: {Username}", loginDto.Username);

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid credentials"
                });
            }

            user.UpdateLastLogin();
            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var token = GenerateJwtToken(user.Id, user.Username, user.Role.Value);
            var expirationSeconds = int.Parse(_configuration.GetSection("JwtSettings")["ExpirationMinutes"] ?? "60") * 60;

            return OkResult(new AuthResponseDto
            {
                Success = true,
                Token = token,
                ExpiresIn = expirationSeconds,
                Role = user.Role.Value
            }, "Login exitoso");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al realizar login");
            return InternalServerErrorResult("Error al realizar el login");
        }
    }

    /// <summary>
    /// Verificar token JWT
    /// </summary>
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult VerifyToken([FromBody] string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? "your-secret-key-min-32-characters-long!";
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            return OkResult(null, "Token válido");
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Token inválido");
            return BadRequestResult("Token inválido o expirado");
        }
    }

    /// <summary>
    /// Obtener información del usuario autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            var username = User.Identity?.Name ?? "Unknown";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Standard";

            return OkResult(new
            {
                userId,
                username,
                role
            }, "Información del usuario");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener información del usuario");
            return InternalServerErrorResult("Error al obtener información del usuario");
        }
    }

    /// <summary>
    /// Obtener listado de usuarios del sistema
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new SystemGym.Application.Features.SystemUsers.Queries.GetSystemUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            var result = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Usuarios obtenidos: {Count} items", result.Data.Count);

            return OkResult(result, "Usuarios obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener usuarios");
            return InternalServerErrorResult("Error al obtener los usuarios");
        }
    }

    /// <summary>
    /// Obtener un usuario específico
    /// </summary>
    [HttpGet("users/{userId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new SystemGym.Application.Features.SystemUsers.Queries.GetSystemUserQuery
            {
                UserId = userId
            };

            var result = await Mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFoundResult("Usuario no encontrado");

            return OkResult(result, "Usuario obtenido exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener usuario {UserId}", userId);
            return InternalServerErrorResult("Error al obtener el usuario");
        }
    }

    /// <summary>
    /// Eliminar usuario del sistema
    /// </summary>
    [HttpDelete("users/{userId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                return NotFoundResult("Usuario no encontrado");

            _unitOfWork.SystemUsers.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Usuario eliminado: {UserId}", userId);

            return OkResult(null, "Usuario eliminado exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al eliminar usuario {UserId}", userId);
            return InternalServerErrorResult("Error al eliminar el usuario");
        }
    }

    /// <summary>
    /// Generar JWT Token
    /// </summary>
    private string GenerateJwtToken(Guid userId, string username, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? "your-secret-key-min-32-characters-long!";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var key = Encoding.ASCII.GetBytes(secret);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("username", username),
                new Claim("role", role),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
