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
            CreateProjection<User, UserModel>()
                .ForMember(d => d.FollowerCount, m => m.MapFrom(s => s.Followers.Count()))
                .ForMember(d => d.PostCount, m => m.MapFrom(s => s.Posts.Count()));
            CreateMap<User, UserMiniModel>();
            CreateMap<Avatar, MetadataModel>().ReverseMap();

            // Post
            CreateMap<CreatePostModel, Post>();
            CreateMap<Post, PostModel>();
            CreateMap<PostFile, MetadataModel>().ReverseMap();

            // Comment
            CreateMap<CreateCommentModel, Comment>();
            CreateMap<Comment, CommentModel>();

            // Attach
            CreateMap<Attach, MetadataModel>();
        }
    }
}