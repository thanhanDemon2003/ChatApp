using Chat.Domain.Entities;

namespace ChatApp.Web.ViewModels
{
    public class ProfileVM
    {
        public ApplicationUser? UserProfile { get; set; }
        public IFormFile? Image { get; set; }
    }
}
