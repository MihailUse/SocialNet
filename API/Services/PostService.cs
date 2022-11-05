using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<Post> GetPosts()
        {
            return _dataContext.Posts.AsNoTracking();
        }

        public IQueryable<Post> GetPostsByAuthor(Guid userId)
        {
            return _dataContext.Posts.Where(x => x.Author.Id == userId).AsNoTracking();
        }

        public async Task<Guid> CreatePost(Post post)
        {
            await _dataContext.AddAsync(post);
            await _dataContext.SaveChangesAsync();
            return post.Id;
        }

        public async Task<Post> GetPostById(Guid id)
        {
            return await _dataContext.Posts.SingleAsync(post => post.Id == id);
        }
    }
}
