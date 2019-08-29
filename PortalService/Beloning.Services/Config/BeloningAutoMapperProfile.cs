using AutoMapper;
using Beloning.Model;
using Beloning.Services.Model;

namespace Beloning.Services.Config
{
    public class BeloningAutoMapperProfile : Profile
    {
        public BeloningAutoMapperProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
