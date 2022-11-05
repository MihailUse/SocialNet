using Common;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly DataContext _dataContext;

        public UserService(DataContext context)
        {
            _dataContext = context;
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

        public async Task<bool> IsEmailExists(string email)
        {
            return await _dataContext.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
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
    }
}