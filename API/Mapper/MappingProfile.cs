using API.MappingActions;
using API.Models.Attach;
using API.Models.Comment;
using API.Models.Post;
using API.Models.Tag;
using API.Models.User;
using AutoMapper;
using Common;
using DAL.Entities;

namespace API.Mapper
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // ProjectionGeneratorService должен содержать идентичные свойства
            Func<Attach, string?>? AttachLinkGenerator = null;
            Func<Avatar, string?>? AvatarLinkGenerator = null;
            Func<PostAttach, string?>? PostAttachLinkGenerator = null;
            Guid RequestUserId = Guid.Empty;

            #region define Maps
            // User
            CreateMap<User, UserModel>();
            CreateMap<CreateUserModel, User>()
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)));

            // Post
            CreateMap<CreatePostModel, Post>();

            // Comment
            CreateMap<CreateCommentModel, Comment>();

            // Attach
            CreateMap<MetadataModel, Avatar>();
            CreateMap<MetadataModel, PostAttach>();
            CreateMap<Avatar, LinkMetadataModel>().AfterMap<AvatarAttachMappingAction>();
            #endregion


            #region define Projections
            // User
            CreateProjection<User, SearchListUserModel>()
                .ForMember(d => d.FollowerCount, m => m.MapFrom(s => s.Followers!.Count));
            CreateProjection<User, UserProfileModel>()
                .ForMember(d => d.PostCount, m => m.MapFrom(s => s.Posts!.Count))
                .ForMember(d => d.FollowerCount, m => m.MapFrom(s => s.Followers!.Count))
                .ForMember(d => d.FollowingCount, m => m.MapFrom(s => s.Followings!.Count));

            // Post
            CreateProjection<Post, PostModel>()
                .ForMember(d => d.IsLiked, m => m.MapFrom(s => s.Likes!.Any(x => x.UserId == RequestUserId)))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.Likes!.Count))
                .ForMember(d => d.CommentCount, m => m.MapFrom(s => s.Comments!.Count))
                .ForMember(d => d.PopularComment, m => m.MapFrom(s => s.Comments!.OrderByDescending(x => x.CommentLikes!.Count).First()));

            // Comment
            CreateProjection<Comment, CommentModel>()
                .ForMember(d => d.IsLiked, m => m.MapFrom(s => s.CommentLikes!.Any(x => x.UserId == RequestUserId)))
                .ForMember(d => d.Text, m => m.MapFrom(s => s.DeletedAt.HasValue ? "Comment has been deleted" : s.Text))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.CommentLikes!.Count));

            // Attach
            // при использовании ProjectTo(config, new { ... }), вторым аргументом следует передавать объект с идентичными свойсвами
            // в рамках проекта следует передавать экземпл€р ProjectionGeneratorService
            CreateProjection<Attach, LinkMetadataModel>()
                .ForMember(d => d.Link, m => m.MapFrom(s => AttachLinkGenerator == null ? null : AttachLinkGenerator(s)));
            CreateProjection<Avatar, LinkMetadataModel>()
                .ForMember(d => d.Link, m => m.MapFrom(s => AvatarLinkGenerator == null ? null : AvatarLinkGenerator(s)));
            CreateProjection<PostAttach, LinkMetadataModel>()
                .ForMember(d => d.Link, m => m.MapFrom(s => PostAttachLinkGenerator == null ? null : PostAttachLinkGenerator(s)));

            // Tag
            CreateProjection<Tag, TagModel>()
                .ForMember(d => d.IsFollowed, m => m.MapFrom(s => s.UserTags!.Any(x => x.UserId == RequestUserId)))
                .ForMember(d => d.PostCount, m => m.MapFrom(s => s.PostTags!.Count))
                .ForMember(d => d.FollowerCount, m => m.MapFrom(s => s.UserTags!.Count));
            #endregion
        }
    }
}