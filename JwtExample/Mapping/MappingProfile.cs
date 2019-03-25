using AutoMapper;
using JwtExample.Data;
using JwtExample.DTOs;

namespace JwtExample.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, RegisterDTO>();
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}
