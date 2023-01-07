﻿using API.Exceptions;
using API.Models.Post;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly TagService _tagService;
        private readonly DataContext _dataContext;
        private readonly AttachService _attachService;
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public PostService(IMapper mapper, TagService tagService, DataContext dataContext, AttachService attachService, ProjectionGeneratorService projectionGeneratorService)
        {
            _mapper = mapper;
            _tagService = tagService;
            _dataContext = dataContext;
            _attachService = attachService;
            _projectionGeneratorService = projectionGeneratorService;
        }

        public IEnumerable<PostModel> GetPosts(int skip, int take)
        {
            return _dataContext.Posts
                .ProjectTo<PostModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public IEnumerable<PostModel> GetPersonalPosts(Guid userId, int skip, int take)
        {
            return _dataContext.Posts
                .Where(x =>
                    x.Author.Followings!.Where(f => f.FollowingId == userId).Any() ||
                    x.Tags!.Where(t => t.Tag.UserTags!.Where(f => f.UserId == userId).Any()).Any()
                )
                .ProjectTo<PostModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public IEnumerable<PostModel> GetUserPosts(Guid userId, int skip, int take)
        {
            return _dataContext.Posts
                .Where(x => x.Author.Id == userId)
                .ProjectTo<PostModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public async Task<Guid> CreatePost(Guid authorId, CreatePostModel createModel)
        {
            Post post = _mapper.Map<Post>(createModel);
            post.AuthorId = authorId;
            post.Tags = new List<PostTag>();

            // add attaches
            if (post.Attaches != null)
            {
                Parallel.ForEach(post.Attaches, postAttach =>
                {
                    postAttach.AuthorId = authorId;
                    _attachService.SaveAttach(postAttach.Id);
                });
            }

            // add tags
            foreach (var tag in await _tagService.ParseTags(post.Text))
                post.Tags.Add(new PostTag() { Tag = tag });

            await _dataContext.AddAsync(post);
            await _dataContext.SaveChangesAsync();
            return post.Id;
        }

        public async Task<Post> GetPostById(Guid postId)
        {
            Post? post = await _dataContext.Posts.FirstOrDefaultAsync(post => post.Id == postId);

            if (post == null)
                throw new NotFoundServiceException("Post not found");

            return post;
        }

        public IEnumerable<PostModel> GetPostsByTag(Guid tagId, int skip, int take)
        {
            return _dataContext.Posts
                .Where(x => x.Tags!.Any(t => t.TagId == tagId))
                .ProjectTo<PostModel>(_mapper.ConfigurationProvider, _projectionGeneratorService)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .AsEnumerable();
        }

        public async Task DeletePost(Guid userId, Guid postId)
        {
            Post post = await GetPostById(postId);

            if (post.AuthorId != userId)
                throw new AccessDeniedServiceException("Access denied");

            _dataContext.Posts.Remove(post);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<PostAttach> GetPostAttach(Guid postId, Guid attachId)
        {
            PostAttach? postFile = await _dataContext.PostAttaches
                .FirstOrDefaultAsync(x => x.PostId == postId && x.Id == attachId);

            if (postFile == null)
                throw new NotFoundServiceException("Attach not found");

            return postFile;
        }

        public async Task<bool> ChangeLikeStatus(Guid userId, Guid postId)
        {
            if (!await CheckPostExists(postId))
                throw new NotFoundServiceException("Post not found");

            PostLike? postLike = await _dataContext.PostLikes
                .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);

            if (postLike == null)
                _dataContext.PostLikes.Add(new PostLike(userId, postId));
            else
                _dataContext.PostLikes.Remove(postLike);

            await _dataContext.SaveChangesAsync();
            return postLike == null;
        }

        private async Task<bool> CheckPostExists(Guid postId)
        {
            return await _dataContext.Posts
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Id == postId);
        }
    }
}
