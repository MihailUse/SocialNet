using API.Models;
using AutoMapper;

namespace API;

public class MappingProfile : Profile
{
    private MappingProfile()
    {
        CreateMap<CreateUserModel, DAL.Entities.User>();
    }
}