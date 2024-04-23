using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Entities
{
    public class Attachment
    {

        public int AttachmentId { get; set; }
        public int MessageId { get; set; }
        public string senderId { get; set; }
        public int conversationId { get; set; }
        public Conversation Conversation { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
