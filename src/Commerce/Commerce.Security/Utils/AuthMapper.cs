using AutoMapper;
using Commerce.Security.DTOs;
using Commerce.Security.Models;

namespace Commerce.Security.Utils;

public class AuthMapper : Profile
{
    public AuthMapper()
    {
        CreateMap<CreateUser, User>();
        CreateMap<User, ReadUser>()
            .ConstructUsing(x => new ReadUser(x.FirstName, x.LastName));
    }
}