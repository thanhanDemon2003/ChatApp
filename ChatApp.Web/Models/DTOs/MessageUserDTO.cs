namespace ChatApp.Web.Models.DTOs
{
    public class MessageUserDTO
    {
        public string senderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? AttachmentType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
