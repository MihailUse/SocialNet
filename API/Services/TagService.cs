using API.Exceptions;
using API.Models.Tag;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace API.Services
{
    public class TagService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public TagService(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<TagInfoModel> GetTagInfoModel(Guid tagId)
        {
            TagInfoModel? tag = await _dataContext.Tags
                .ProjectTo<TagInfoModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == tagId);

            if (tag == null)
                throw new NotFoundServiceException("Tag not found");

            return tag;
        }

        public IEnumerable<SearchTagModel> SearchTags(string search, int take)
        {
            return _dataContext.Tags
                .Where(x => EF.Functions.ILike(x.Name, $"{search}%"))
                .ProjectTo<SearchTagModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.FollowerCount)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public async Task ChangeFollowStatus(Guid userId, Guid tagId)
        {
            UserTag? tag = await _dataContext.UserTags
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TagId == tagId);

            if (tag == null)
                _dataContext.UserTags.Add(new UserTag(tagId, userId));
            else
                _dataContext.UserTags.Remove(tag);

            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Tag>> ParseTags(string text)
        {
            Regex regex = new Regex(@"\B(#+[a-zA-Z0-9(_)]{1,})");
            MatchCollection matches = regex.Matches(text);

            return await AddIfNotExistsRange(matches.Select(x => x.Value));
        }

        private async Task<List<Tag>> AddIfNotExistsRange(IEnumerable<string> names)
        {
            List<Tag> tags = new List<Tag>();

            // add uniques tags
            foreach (string name in names.Distinct())
                tags.Add(await AddIfNotExists(name));

            await _dataContext.SaveChangesAsync();
            return tags;
        }

        private async Task<Tag> AddIfNotExists(string name)
        {
            Tag tag = await _dataContext.Tags.FirstOrDefaultAsync(x => x.Name == name) ?? new Tag(name);

            if (tag.Id == default)
                await _dataContext.Tags.AddAsync(tag);

            return tag;
        }
    }
}
