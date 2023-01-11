using API.Models.User;
using API.Services;
using AutoMapper;
using DAL.Entities;

namespace API.MappingActions
{
    public class UserModelMappingAction : IMappingAction<User, UserModel>
    {
        private readonly ProjectionGeneratorService _urlService;

        public UserModelMappingAction(ProjectionGeneratorService urlService)
        {
            _urlService = urlService;
        }

        void IMappingAction<User, UserModel>.Process(User source, UserModel destination, ResolutionContext context)
        {
            destination.AvatarLink = _urlService.AvatarLinkGenerator?.Invoke(source);
        }
    }
}
