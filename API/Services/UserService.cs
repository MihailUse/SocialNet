using API.Models.Attach;
using Common;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly DataContext _dataContext;
        private readonly AttachService _attachService;

        public UserService(DataContext context, AttachService attachService)
        {
            _dataContext = context;
            _attachService = attachService;
        }

        public async Task<Guid> CreateUser(User user)
        {
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User> UpdateUser(Guid id, User userOptions)
        {
            User user = await GetUserById(id);

            // TODO: refactor this
            user.Nickname = userOptions.Nickname ?? user.Nickname;
            user.FullName = userOptions.FullName ?? user.FullName;
            user.About = userOptions.About ?? user.About;

            await _dataContext.SaveChangesAsync();
            return user;
        }

        public IQueryable<User> GetUsers()
        {
            return _dataContext.Users.AsNoTracking();
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _dataContext.Users.SingleAsync(x => x.Id == id);
        }

        private async Task<User> GetUserByEmail(string email)
        {
            return await _dataContext.Users.SingleAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task DeleteUser(Guid id)
        {
            User user = await GetUserById(id);
            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByCredential(string email, string password)
        {
            User user = await GetUserByEmail(email);

            if (!HashHelper.Verify(user.PasswordHash, password))
                throw new Exception("Password is incorrect");

            return user;
        }

        public async Task<bool> IsNicknameExists(string nickname)
        {
            return await _dataContext.Users.AnyAsync(x => x.Nickname == nickname);
        }

        public async Task ChangeFollowStatus(Guid followerId, Guid followingId)
        {
            User following = await GetUserById(followingId);

            Follower? followerExists = await _dataContext.Followers
                .FirstOrDefaultAsync(x => x.FollewerId == followerId && x.FollowingId == following.Id);

            if (followerExists == null)
                _dataContext.Followers.Add(new Follower(followerId, followingId));
            else
                _dataContext.Followers.Remove(followerExists);

            await _dataContext.SaveChangesAsync();
        }

        public async Task SetUserAvatar(Guid userId, MetadataModel meta)
        {
            User user = await _dataContext.Users.Include(x => x.Avatar).SingleAsync(x => x.Id == userId);
            Avatar avatar = new Avatar
            {
                Author = user,
                MimeType = meta.MimeType,
                Name = meta.Name,
                Size = meta.Size
            };

            _attachService.SaveAttach(meta.Id);

            user.Avatar = avatar;
            await _dataContext.SaveChangesAsync();
        }
    }
}