using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CommentService
    {
        private readonly DataContext _dataContext;
        private readonly PostService _postService;

        public CommentService(DataContext dataContext, PostService postService)
        {
            _dataContext = dataContext;
            _postService = postService;
        }

        public IQueryable<Comment> GetCommentsByPost(Guid postId)
        {
            return _dataContext.Comments
                .Where(x => x.PostId == postId)
                .IgnoreQueryFilters();
        }

        public async Task<Guid> CreateComment(Comment comment)
        {
            Post post = await _postService.GetPostById(comment.PostId);

            if (!post.IsCommentable)
                throw new Exception("Post is not commentable");

            await _dataContext.AddAsync(comment);
            await _dataContext.SaveChangesAsync();
            return comment.Id;
        }

        public async Task DeleteComment(Guid userId, Guid commentId)
        {
            Comment comment = await GetCommentById(commentId);
            Post post = await _postService.GetPostById(comment.PostId);

            if (comment.AuthorId != userId || post.AuthorId != userId)
                throw new Exception("User no permission");

            _dataContext.Comments.Remove(comment);
            await _dataContext.SaveChangesAsync();
        }

        private async Task<Comment> GetCommentById(Guid commentId)
        {
            Comment? comment = await _dataContext.Comments.FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
                throw new Exception("Comment not found");

            return comment;
        }
    }
}
