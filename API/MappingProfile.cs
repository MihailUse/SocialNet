using API.Models.Attach;
using API.Models.Comment;
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
            CreateMap<MetadataModel, Avatar>();

            // Post
            CreateMap<CreatePostModel, Post>();
            CreateMap<Post, PostModel>();
            CreateMap<MetadataModel, PostFile>();
            CreateMap<PostFile, MetadataModel>();

            // Comment
            CreateMap<CreateCommentModel, Comment>();
            CreateMap<Comment, CommentModel>();

            // Attach
            CreateMap<Attach, MetadataModel>();
        }
    }
}