namespace SystemGym.Application.Mappings;

using AutoMapper;
using SystemGym.Application.DTOs.Auth;
using SystemGym.Application.DTOs.Clients;
using SystemGym.Application.DTOs.Plans;
using SystemGym.Application.DTOs.Products;
using SystemGym.Application.DTOs.Sales;
using SystemGym.Application.DTOs.Subscriptions;
using SystemGym.Application.DTOs.SystemUsers;
using SystemGym.Application.Features.Inventory.Queries;
using SystemGym.Domain.Entities;

/// <summary>
/// Perfil de AutoMapper para mapeos de SystemUser
/// </summary>
public class SystemUserMappingProfile : Profile
{
    public SystemUserMappingProfile()
    {
        CreateMap<SystemUser, SystemUserResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Habilitado));

        CreateMap<CreateSystemUserDto, SystemUser>()
            .ConstructUsing((src, ctx) => SystemUser.Create(src.Username, src.Password, string.Empty, src.Role));
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de Client
/// </summary>
public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        CreateMap<Client, ClientResponseDto>()
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Habilitado));

        CreateMap<CreateClientDto, Client>()
            .ConstructUsing((src, ctx) => Client.Create(
                src.TipoDocumento,
                src.NumeroDocumento,
                src.NombreCompleto,
                src.Celular));

        CreateMap<UpdateClientDto, Client>();
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de Product
/// </summary>
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Habilitado));

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de Plan
/// </summary>
public class PlanMappingProfile : Profile
{
    public PlanMappingProfile()
    {
        CreateMap<Plan, PlanResponseDto>()
            .ForMember(dest => dest.PlanId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Habilitado));

        CreateMap<CreatePlanDto, Plan>();
        CreateMap<UpdatePlanDto, Plan>();
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de Subscription
/// </summary>
public class SubscriptionMappingProfile : Profile
{
    public SubscriptionMappingProfile()
    {
        CreateMap<Subscription, SubscriptionResponseDto>()
            .ForMember(dest => dest.SubscriptionId, opt => opt.MapFrom(src => src.Id));

        CreateMap<CreateSubscriptionDto, Subscription>();
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de SalesHistory
/// </summary>
public class SalesMappingProfile : Profile
{
    public SalesMappingProfile()
    {
        CreateMap<SalesHistory, SaleResponseDto>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id));

        // Nota: CreateMap para CreateSaleDto → SalesHistory se maneja en el command handler
        // ya que necesitamos el userId del contexto autenticado
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de InventoryLog
/// </summary>
public class InventoryMappingProfile : Profile
{
    public InventoryMappingProfile()
    {
        CreateMap<InventoryLog, InventoryLogDto>()
            .ForMember(dest => dest.LogId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CantidadAnterior, opt => opt.MapFrom(src => src.CantidadAnterior))
            .ForMember(dest => dest.CantidadNueva, opt => opt.MapFrom(src => src.CantidadNueva))
            .ForMember(dest => dest.Diferencia, opt => opt.MapFrom(src => src.Diferencia))
            .ForMember(dest => dest.Operacion, opt => opt.MapFrom(src => src.Operacion))
            .ForMember(dest => dest.MotivoAuditoria, opt => opt.MapFrom(src => src.Razon ?? string.Empty))
            .ForMember(dest => dest.ProductoDescripcion, opt => opt.Ignore());
    }
}
