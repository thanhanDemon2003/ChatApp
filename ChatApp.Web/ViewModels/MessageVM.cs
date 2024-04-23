namespace ChatApp.Web.ViewModels
{
    public class MessageVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
    }
}
