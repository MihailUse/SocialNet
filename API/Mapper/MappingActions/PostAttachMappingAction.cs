using API.Models.Attach;
using API.Services;
using AutoMapper;
using DAL.Entities;

namespace API.MappingActions
{
    public class PostAttachMappingAction : IMappingAction<PostAttach, LinkMetadataModel>
    {
        private readonly ProjectionGeneratorService _urlService;

        public PostAttachMappingAction(ProjectionGeneratorService urlService)
        {
            _urlService = urlService;
        }

        public void Process(PostAttach source, LinkMetadataModel destination, ResolutionContext context)
        {
            destination.Link = _urlService.PostAttachLinkGenerator?.Invoke(source);
        }
    }
}
