using DAL.Entities;

namespace API.Services
{
    public class LinkGeneratorService
    {
        public Func<Attach, string?>? AttachLinkGenerator { get; set; }
        public Func<Avatar, string?>? AvatarLinkGenerator { get; set; }
        public Func<PostAttach, string?>? PostFileLinkGenerator { get; set; }
    }
}
