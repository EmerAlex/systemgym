namespace SystemGym.Application.Abstractions;

using MediatR;

/// <summary>
/// Interfaz base para comandos
/// </summary>
public interface ICommand : IRequest<CommandResult> { }

/// <summary>
/// Interfaz base para comandos genéricos
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Resultado genérico de un comando
/// </summary>
public class CommandResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static CommandResult<T> SuccessResult(T? data = default, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static CommandResult<T> FailureResult(string message, Dictionary<string, string[]>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

/// <summary>
/// Resultado estándar de un comando (usar para retornar object)
/// </summary>
public class CommandResult : CommandResult<object>
{
    public static new CommandResult SuccessResult(object? data = null, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static new CommandResult FailureResult(string message, Dictionary<string, string[]>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

/// <summary>
/// Interfaz base para queries
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Interfaz base para command handlers
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, CommandResult> where TCommand : ICommand { }

/// <summary>
/// Interfaz base para command handlers con respuesta genérica
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse> { }

/// <summary>
/// Interfaz base para query handlers
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse> 
    where TQuery : IQuery<TResponse> { }
