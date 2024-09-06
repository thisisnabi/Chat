using Chat.APIs.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chat.APIs.SharedData
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> dbContextOptions)
            : base(dbContextOptions)
        { 
            
        }

        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>().HasKey(d => d.Id);
            modelBuilder.Entity<Conversation>().Property(x => x.Title).IsRequired();

            modelBuilder.Entity<Conversation>().OwnsMany<Message>(x => x.Messages ,builderMessages =>
            {
                builderMessages.HasKey(z => z.Id);
                builderMessages.Property(z => z.Content)
                               .HasMaxLength(400)
                               .IsRequired();

                builderMessages.Property(z => z.Username)
                               .IsRequired();
            });

            modelBuilder.Entity<Conversation>().OwnsMany<ConversationUser>(x => x.Users, builderUer =>
            {
                builderUer.HasKey(z => z.Id);
                builderUer.Property(z => z.UserName)
                               .HasMaxLength(400)
                               .IsRequired();

                builderUer.Property(z => z.ConversationId)
                               .IsRequired();
            });

        }


    }
}
