using AutoMapper;
using Dtos;
using Models;

namespace Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //Source -> Destination
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}