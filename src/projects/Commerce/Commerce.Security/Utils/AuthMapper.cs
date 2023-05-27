using AutoMapper;
using Commerce.Security.DTOs;
using Commerce.Security.Models;

namespace Commerce.Security.Utils;

public class AuthMapper : Profile
{
    public AuthMapper()
    {
        CreateMap<CreateUser, User>()
            .ConstructUsing(src => new User(src.FirstName, src.LastName, src.Email, src.Password))
            .ForMember(dest => dest.EmailAddress,
                opt => opt.MapFrom(src => src.Email));
        
        CreateMap<User, ReadUser>()
            .ConstructUsing(x => new ReadUser(x.FirstName, x.LastName));
    }
}