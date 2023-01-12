using API.Exceptions;
using API.Models.Comment;
using API.Models.Notification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CommentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly PostService _postService;
        private readonly NotificationService _notificationService;
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public CommentService(
            IMapper mapper,
            DataContext dataContext,
            PostService postService,
            NotificationService notificationService,
            ProjectionGeneratorService projectionGeneratorService
        )
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _postService = postService;
            _notificationService = notificationService;
            _projectionGeneratorService = projectionGeneratorService;
        }

        public IEnumerable<CommentModel> GetPostComments(Guid postId, int skip, int take)
        {
            return _dataContext.Comments
                .Where(x => x.PostId == postId)
                .IgnoreQueryFilters()
                .ProjectTo<CommentModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public async Task<Guid> CreateComment(Guid userId, CreateCommentModel createModel)
        {
            Comment comment = _mapper.Map<Comment>(createModel);
            Post post = await _postService.GetPostById(comment.PostId);
            comment.AuthorId = userId;

            if (!post.IsCommentable)
                throw new InvalidParameterServiceException("Post is not commentable");

            await _dataContext.AddAsync(comment);
            await _dataContext.SaveChangesAsync();
            return comment.Id;
        }

        public async Task DeleteComment(Guid userId, Guid commentId)
        {
            Comment comment = await GetCommentPostById(commentId);

            if (comment.AuthorId != userId || comment.Post.AuthorId != userId)
                throw new AccessDeniedServiceException("Access denied");

            _dataContext.Comments.Remove(comment);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> ChangeLikeStatus(Guid userId, Guid commentId)
        {
            if (!await CheckCommentExists(commentId))
                throw new NotFoundServiceException("Comment not found");

            CommentLike? commentLike = await _dataContext.CommentLikes
                .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);

            if (commentLike == null)
            {
                commentLike = new CommentLike(userId, commentId);
                await _dataContext.CommentLikes.AddAsync(commentLike);
                await _dataContext.SaveChangesAsync();
                await SendNotification(userId, commentId);
                return true;
            }

            _dataContext.CommentLikes.Remove(commentLike);
            await _dataContext.SaveChangesAsync();
            return false;
        }

        private async Task SendNotification(Guid userId, Guid commentId)
        {
            CommentLike postLike = await _dataContext.CommentLikes
                .Include(x => x.Comment)
                .Include(x => x.User)
                .FirstAsync(x => x.UserId == userId && x.CommentId == commentId);

            await _notificationService.CreateNotification(
                userId,
                postLike.Comment.AuthorId,
                NotificationType.LikeComment,
                new AlertModel("Liked comment", null, $"@{postLike.User.Nickname} liked your comment")
            );
        }

        private async Task<bool> CheckCommentExists(Guid commentId)
        {
            return await _dataContext.Comments
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Id == commentId);
        }

        private async Task<Comment> GetCommentPostById(Guid commentId)
        {
            Comment? comment = await _dataContext.Comments
                .Include(x => x.Post)
                .Where(x => x.Id == commentId)
                .FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundServiceException("Comment not found");

            return comment;
        }
    }
}
