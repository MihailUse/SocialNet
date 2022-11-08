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

        public IQueryable<User> GetUsers()
        {
            return _dataContext.Users
                .Include(x => x.Avatar)
                .AsNoTracking();
        }

        public async Task<User> GetUserById(Guid id)
        {
            User? user = await _dataContext.Users
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                throw new Exception("User not found");

            return user;
        }

        private async Task<User> GetUserByEmail(string email)
        {
            User? user = await _dataContext.Users
                .Include(x => x.Avatar)
                .SingleAsync(x => x.Email.ToLower() == email.ToLower());

            if (user == null)
                throw new Exception("User not found");

            return user;
        }

        public async Task<Avatar> GetUserAvatar(Guid id)
        {
            Avatar? avatar = await _dataContext.Avatars.FirstOrDefaultAsync(x => x.UserId == id);

            if (avatar == null)
                throw new Exception("Avatar not found");

            return avatar;
        }

        public IQueryable<Attach> GetUserAttaches(Guid id)
        {
            return _dataContext.Attaches
                .Where(x => x.AuthorId == id)
                .AsNoTracking();
        }

        public async Task<Guid> CreateUser(User user)
        {
            if (await IsEmailExists(user.Email))
                throw new Exception("Email already exists");

            if (await IsNicknameExists(user.Nickname))
                throw new Exception("Nickname already exists");

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User> UpdateUser(Guid id, User userOptions)
        {
            User user = await GetUserById(id);

            user.Nickname = userOptions.Nickname ?? user.Nickname;
            user.FullName = userOptions.FullName ?? user.FullName;
            user.About = userOptions.About ?? user.About;

            await _dataContext.SaveChangesAsync();
            return user;
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

        public async Task<bool> IsEmailExists(string email)
        {
            return await _dataContext.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
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
            User user = await GetUserById(userId);
            user.Avatar = new Avatar
            {
                Id = meta.Id,
                AuthorId = userId,
                MimeType = meta.MimeType,
                Name = meta.Name,
                Size = meta.Size
            };

            _attachService.SaveAttach(meta.Id);
            await _dataContext.SaveChangesAsync();
        }
    }
}