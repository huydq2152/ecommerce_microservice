using AutoMapper;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Infrastructure.ViewModels;

namespace IdentityServer;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Permission, PermissionViewModel>();
        CreateMap<Permission, PermissionUserViewModel>();
    }
}