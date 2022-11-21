using API.Models.Attach;
using API.Services;
using AutoMapper;
using DAL.Entities;

namespace API.Mapper.MappingActions
{
    public class PostAttachMappingAction : IMappingAction<PostAttach, LinkMetadataModel>
    {
        private readonly LinkGeneratorService _urlService;

        public PostAttachMappingAction(LinkGeneratorService urlService)
        {
            _urlService = urlService;
        }

        public void Process(PostAttach source, LinkMetadataModel destination, ResolutionContext context)
        {
            destination.Link = _urlService.PostFileLinkGenerator?.Invoke(source);
        }
    }
}
