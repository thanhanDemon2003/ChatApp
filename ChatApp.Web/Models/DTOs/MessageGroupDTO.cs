using System.ComponentModel.DataAnnotations;

namespace ChatApp.Web.Models.DTOs
{
    public class MessageGroupDTO
    {
        public string senderId { get; set; }
        public int? GroupId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? AttachmentType { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
