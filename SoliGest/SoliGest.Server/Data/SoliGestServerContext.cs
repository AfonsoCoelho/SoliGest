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
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }


        //public DbSet<SoliGest.Server.Models.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
} 
