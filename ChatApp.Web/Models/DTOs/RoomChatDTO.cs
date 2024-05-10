using System.ComponentModel.DataAnnotations;

namespace ChatApp.Web.Models.DTOs
{
    public class RoomChatDTO
    {
        [Required]
        public string RoomName { get; set; }
        [Required]
        public string userCreate { get; set; }
        [Required]
        public List<string> userIds { get; set; }
    }
}
