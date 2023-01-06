using DAL.Entities;

namespace API.Services
{
    public class ProjectionGeneratorService
    {
        public Func<Attach, string?>? AttachLinkGenerator { get; set; }
        public Func<Avatar, string?>? AvatarLinkGenerator { get; set; }
        public Func<PostAttach, string?>? PostAttachLinkGenerator { get; set; }
        public Guid RequestUserId { get; set; }
    }
}
