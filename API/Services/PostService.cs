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

        public IQueryable<Post> GetPosts()
        {
            return _dataContext.Posts
                .Include(x => x.Files)
                .OrderBy(x => x.CreatedAt)
                .AsNoTracking();
        }

        public IQueryable<Post> GetPostsByAuthor(Guid userId)
        {
            return _dataContext.Posts
                .Include(x => x.Files)
                .Where(x => x.Author.Id == userId)
                .AsNoTracking();
        }

        public async Task<Post> GetPostById(Guid id)
        {
            return await _dataContext.Posts
                .Include(x => x.Files)
                .SingleAsync(post => post.Id == id);
        }

        public async Task<Guid> CreatePost(Guid userId, Post post)
        {
            if (post.Files != null)
            {
                List<PostFile> files = post.Files.ToList();

                for (int i = 0; i < files.Count; i++)
                {
                    files[i].AuthorId = userId;
                    _attachService.SaveAttach(files[i].Id);
                }

                post.AuthorId = userId;
            }

            await _dataContext.AddAsync(post);
            await _dataContext.SaveChangesAsync();
            return post.Id;
        }
    }
}
