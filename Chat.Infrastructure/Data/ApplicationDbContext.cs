using Chat.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Conversation> Conversations { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
        public DbSet<UserConversation> UserConversations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>().ToTable("Messages");
            builder.Entity<Conversation>().ToTable("Conversations");
            builder.Entity<UserConversation>().ToTable("UserConversations");
            builder.Entity<Attachment>().ToTable("Attachments");

        }

    }
}
