using API.Exceptions;
using API.Models.Attach;
using API.Models.Notification;
using API.Models.User;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly AttachService _attachService;
        private readonly NotificationService _notificationService;
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public UserService(
            IMapper mapper,
            DataContext context,
            AttachService attachService,
            NotificationService notificationService,
            ProjectionGeneratorService projectionGeneratorService
        )
        {
            _mapper = mapper;
            _dataContext = context;
            _attachService = attachService;
            _notificationService = notificationService;
            _projectionGeneratorService = projectionGeneratorService;
        }

        // for testing
        public IEnumerable<UserModel> GetUsers()
        {
            return _dataContext.Users
                .Include(x => x.Avatar)
                .ProjectTo<UserModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AsEnumerable();
        }

        public IEnumerable<MetadataModel> GetUserAttaches(Guid userId)
        {
            return _dataContext.Attaches
                .Where(x => x.AuthorId == userId)
                .ProjectTo<LinkMetadataModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .AsNoTracking()
                .AsEnumerable();
        }

        public IEnumerable<SearchListUserModel> SearchUsers(string search, int skip, int take)
        {
            return _dataContext.Users
                .Include(x => x.Avatar)
                .Where(x => EF.Functions.ILike(x.Nickname, $"%{search}%") || EF.Functions.ILike(x.FullName!, $"%{search}%"))
                .ProjectTo<SearchListUserModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.FollowerCount)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public async Task<UserProfileModel> GetUserProfile(Guid userId)
        {
            UserProfileModel? user = await _dataContext.Users
                .Include(x => x.Avatar)
                .Where(x => x.Id == userId)
                .ProjectTo<UserProfileModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundServiceException("User not found");

            return user;
        }

        public async Task<Guid> CreateUser(CreateUserModel createModel)
        {
            User user = _mapper.Map<User>(createModel);

            if (await CheckEmailExists(user.Email))
                throw new InvalidParameterServiceException("Email already exists");

            if (await CheckNicknameExists(user.Nickname))
                throw new InvalidParameterServiceException("Nickname already exists");

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<Avatar> GetUserAvatar(Guid userId)
        {
            Avatar? avatar = await _dataContext.Avatars
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (avatar == null)
                throw new NotFoundServiceException("Avatar not found");

            return avatar;
        }

        public async Task<Attach> GetUserAttach(Guid userId, Guid attachId)
        {
            Attach? attach = await _dataContext.Attaches
                .FirstOrDefaultAsync(x => x.Id == attachId);

            if (attach == null)
                throw new NotFoundServiceException("Attach not found");

            if (attach.AuthorId != userId)
                throw new AccessDeniedServiceException("Access denied");

            return attach;
        }

        public async Task UpdateUser(Guid userId, UpdateUserModel userOptions)
        {
            User user = await GetUserById(userId);

            user.Nickname = userOptions.Nickname ?? user.Nickname;
            user.FullName = userOptions.FullName ?? user.FullName;
            user.About = userOptions.About ?? user.About;

            if (await CheckEmailExists(user.Email, userId))
                throw new InvalidParameterServiceException("Email already exists");

            if (await CheckNicknameExists(user.Nickname, userId))
                throw new InvalidParameterServiceException("Nickname already exists");

            if (userOptions.Avatar != null)
            {
                user.Avatar = _mapper.Map<Avatar>(userOptions.Avatar);
                user.Avatar.AuthorId = userId;
                _attachService.SaveAttach(user.Avatar.Id);
            }
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid userId)
        {
            User user = await GetUserById(userId);

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByCredential(string email, string password)
        {
            User user = await GetUserByEmail(email);

            if (!HashHelper.Verify(user.PasswordHash, password))
                throw new AuthException("Password is incorrect");

            return user;
        }

        public async Task<bool> ChangeFollowStatus(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
                throw new InvalidParameterServiceException("User can not to subscribe to yourself");

            User following = await GetUserById(followingId);
            Follower? follower = await _dataContext.Followers
                .FirstOrDefaultAsync(x => x.FollewerId == followerId && x.FollowingId == following.Id);

            if (follower == null)
            {
                follower = new Follower(followerId, followingId);
                await _dataContext.Followers.AddAsync(follower);
                await _dataContext.SaveChangesAsync();

                User? fromUser = await _dataContext.Users.FindAsync(followerId);
                await _notificationService.CreateNotification(
                    followerId,
                    followingId,
                    NotificationType.NewFollower,
                    new AlertModel("New follower", null, $"@{fromUser!.Nickname} has subscribed to you")
                );
                return true;
            }

            _dataContext.Followers.Remove(follower);
            await _dataContext.SaveChangesAsync();
            return false;
        }

        public async Task SetNotificationToken(Guid userId, string? token)
        {
            User user = await GetUserById(userId);
            user.NotificationToken = token;
            await _dataContext.SaveChangesAsync();
        }

        public async Task<string?> GetNotificationToken(Guid userId)
        {
            User user = await GetUserById(userId);
            return user.NotificationToken;
        }

        private async Task<bool> CheckNicknameExists(string nickname, Guid? excludeUserId = null)
        {
            return await _dataContext.Users
                .AnyAsync(x => x.Nickname == nickname && x.Id != excludeUserId);
        }

        private async Task<bool> CheckEmailExists(string email, Guid? excludeUserId = null)
        {
            return await _dataContext.Users
                .AnyAsync(x => x.Email.ToLower() == email.ToLower() && x.Id != excludeUserId);
        }

        private async Task<User> GetUserById(Guid userId)
        {
            User? user = await _dataContext.Users
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new NotFoundServiceException("User not found");

            return user;
        }

        private async Task<User> GetUserByEmail(string email)
        {
            User? user = await _dataContext.Users
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => EF.Functions.ILike(x.Email, $"%{email}%"));

            if (user == null)
                throw new NotFoundServiceException("User not found");

            return user;
        }
    }
}