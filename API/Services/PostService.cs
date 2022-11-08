using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PostService
    {
        private readonly DataContext _dataContext;
        private readonly AttachService _attachService;

        public PostService(DataContext dataContext, AttachService attachService)
        {
            _dataContext = dataContext;
            _attachService = attachService;
        }

        public IQueryable<Post> GetPosts(int skip, int take)
        {
            return _dataContext.Posts
                .Include(x => x.Files)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .Skip(skip)
                .AsNoTracking();
        }

        public IQueryable<Post> GetPostsByAuthor(Guid userId)
        {
            return _dataContext.Posts
                .Include(x => x.Files)
                .Where(x => x.Author.Id == userId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking();
        }

        public async Task<Guid> CreatePost(Post post)
        {

            if (post.Files != null)
            {
                List<PostFile> files = post.Files.ToList();

                for (int i = 0; i < files.Count; i++)
                {
                    files[i].AuthorId = post.AuthorId;
                    _attachService.SaveAttach(files[i].Id);
                }
            }

            await _dataContext.AddAsync(post);
            await _dataContext.SaveChangesAsync();
            return post.Id;
        }

        public async Task<Post> GetPostById(Guid postId)
        {
            Post? post = await _dataContext.Posts.FirstOrDefaultAsync(post => post.Id == postId);

            if (post == null)
                throw new Exception("Post not found");

            return post;
        }

        public async Task DeletePost(Guid userId, Guid postId)
        {
            Post post = await GetPostById(postId);

            if (post.AuthorId != userId)
                throw new Exception("User no permission");

            _dataContext.Posts.Remove(post);
            await _dataContext.SaveChangesAsync();
        }
    }
}
