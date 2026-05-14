# ============================================================
# Stage 1: Build Backend (.NET 10)
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src

# Copiar solo los .csproj primero para aprovechar cache de capas
COPY SystemGym.API/SystemGym.API.csproj                   SystemGym.API/
COPY SystemGym.Application/SystemGym.Application.csproj   SystemGym.Application/
COPY SystemGym.Domain/SystemGym.Domain.csproj             SystemGym.Domain/
COPY SystemGym.Infrastructure/SystemGym.Infrastructure.csproj SystemGym.Infrastructure/

# Restaurar dependencias (cacheado si no cambian los .csproj)
RUN dotnet restore SystemGym.API/SystemGym.API.csproj

# Copiar el resto del código fuente
COPY SystemGym.API/           SystemGym.API/
COPY SystemGym.Application/   SystemGym.Application/
COPY SystemGym.Domain/        SystemGym.Domain/
COPY SystemGym.Infrastructure/ SystemGym.Infrastructure/

# Publicar en Release
WORKDIR /src/SystemGym.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# ============================================================
# Stage 2: Runtime (.NET 10 ASP.NET)
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Instalar curl para health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Crear usuario no-root por seguridad
RUN useradd -m -u 1001 appuser

# Copiar publicación
COPY --from=backend-build /app/publish .

# Asignar permisos
RUN chown -R appuser:appuser /app
USER appuser

# Puerto de escucha
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "SystemGym.API.dll"]
