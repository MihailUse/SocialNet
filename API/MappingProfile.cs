using API.Models.Attach;
using API.Models.Post;
using API.Models.User;
using AutoMapper;
using Common;
using DAL.Entities;

namespace API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<CreateUserModel, User>()
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)));
            CreateMap<UpdateUserModel, User>();
            CreateMap<User, UserModel>();
            CreateMap<Avatar, MetadataModel>();

            // Post
            CreateMap<CreatePostModel, Post>();
            CreateMap<MetadataModel, PostFile>();
            CreateMap<Post, PostModel>();
            CreateMap<PostFile, MetadataModel>();
        }
    }
}