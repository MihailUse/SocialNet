using API.Models.Attach;
using API.Services;
using AutoMapper;
using DAL.Entities;

namespace API.MappingActions
{
    public class AvatarAttachMappingAction : IMappingAction<Avatar, LinkMetadataModel>
    {
        private readonly ProjectionGeneratorService _urlService;

        public AvatarAttachMappingAction(ProjectionGeneratorService urlService)
        {
            _urlService = urlService;
        }

        public void Process(Avatar source, LinkMetadataModel destination, ResolutionContext context)
        {
            destination.Link = _urlService.AvatarLinkGenerator?.Invoke(source.User);
        }
    }
}
