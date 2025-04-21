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

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Contact)
                .WithMany()
                .HasForeignKey(c => c.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 
