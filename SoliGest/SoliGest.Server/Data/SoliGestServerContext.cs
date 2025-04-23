using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Models;

namespace SoliGest.Server.Data
{
    public class SoliGestServerContext : IdentityDbContext<User>
    {
        public SoliGestServerContext(DbContextOptions<SoliGestServerContext> options)
            : base(options)
        {
        }

        public DbSet<SolarPanel> SolarPanel { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<AssistanceRequest> AssistanceRequest { get; set; } = default!;
        public DbSet<DayOff> DaysOff { get; set; }
        public DbSet<Holidays> Holidays { get; set; }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }


        //public DbSet<SoliGest.Server.Models.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //
            // 1) Many-to-Many: Conversation ↔ User
            //
            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Conversations)
                .UsingEntity<Dictionary<string, object>>(
                    "ConversationUser",
                    join => join
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join => join
                        .HasOne<Conversation>()
                        .WithMany()
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("ConversationId", "UserId");
                        join.ToTable("ConversationUsers");
                    }
                );

            //
            // 2) Conversation → Contact (1:N via shadow FK ContactId)
            //
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Contact)
                .WithMany()
                .HasForeignKey("ContactId")
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 3) Message → Sender (User) (1:N)
            //
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 4) Message → Receiver (User) (1:N)
            //
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 5) Message → Conversation (N:1)
            //
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 
