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
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public TagService(IMapper mapper, DataContext dataContext, ProjectionGeneratorService projectionGeneratorService)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _projectionGeneratorService = projectionGeneratorService;
        }

        public IEnumerable<TagModel> SearchTags(string search, int skip, int take)
        {
            return _dataContext.Tags
                .Where(x => EF.Functions.ILike(x.Name, $"{search}%"))
                .ProjectTo<TagModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.FollowerCount)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public async Task<TagModel> GetTagById(Guid tagId)
        {
            TagModel? tag = await _dataContext.Tags
                .Where(x => x.Id == tagId)
                .ProjectTo<TagModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();

            if (tag == null)
                throw new NotFoundServiceException("Tag not found");

            return tag;
        }

        public async Task<bool> ChangeFollowStatus(Guid userId, Guid tagId)
        {
            UserTag? tag = await _dataContext.UserTags
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TagId == tagId);

            if (tag == null)
                _dataContext.UserTags.Add(new UserTag(tagId, userId));
            else
                _dataContext.UserTags.Remove(tag);

            await _dataContext.SaveChangesAsync();
            return tag == null;
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
