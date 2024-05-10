using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ChatApp.Web.Models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [ValidateNever]
        public DateTime? LastSeen { get; set; }
    }
}
