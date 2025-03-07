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
        public SoliGestServerContext (DbContextOptions<SoliGestServerContext> options)
            : base(options)
        {
        }

        public DbSet<SoliGest.Server.Models.SolarPanel> SolarPanel { get; set; } = default!;
        public DbSet<SoliGest.Server.Models.Address> Address { get; set; } = default!;
        public DbSet<SoliGest.Server.Models.AssistanceRequest> AssistanceRequest { get; set; } = default!;
        public DbSet<SoliGest.Server.Models.User> User { get; set; } = default!;
    }
}
