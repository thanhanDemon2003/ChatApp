using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Entities
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public string UserId { get; set; }
        public int? GroupId { get; set; }
        public string ReceiverId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime SentDate { get; set; }
        public bool isDeleted { get; set; } = false;
        public ApplicationUser User { get; set; }
        public Group Group { get; set; }
        public ApplicationUser Receiver { get; set; }
    }
}

