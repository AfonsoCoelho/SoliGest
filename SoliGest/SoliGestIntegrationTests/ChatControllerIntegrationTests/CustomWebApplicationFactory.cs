using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.InMemory;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using Microsoft.AspNetCore.Builder;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SoliGestServerContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);



            services.AddDbContext<SoliGestServerContext>(opts =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                var uniqueName = $"TestDb_{Guid.NewGuid()}";
                opts.UseInMemoryDatabase(uniqueName)
                    .UseInternalServiceProvider(serviceProvider);
            });

            builder.Configure(app =>
            {
                app.UseDeveloperExceptionPage();
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SoliGestServerContext>();
                db.Database.EnsureCreated();

                // Semeia dois utilizadores
                db.Users.AddRange(
                    new User { Id = "test-user", 
                        Name = "Test User1",
                        Address1 = "Rua Principal, 123",
                        Address2 = "Apto 45",
                        BirthDate = new DateTime(1990, 1, 1).ToString(),
                        DayOff = DayOfWeek.Sunday.ToString(),
                        StartHoliday = DateTime.Today.ToString(),
                        EndHoliday = DateTime.Today.AddDays(7).ToString(),
                        Role = "Supervisor"
                    },
                    new User { Id = "1", 
                        Name = "Receiver User",
                        Address1 = "Rua Principal, 123",
                        Address2 = "Apto 45",
                        BirthDate = new DateTime(1990, 1, 1).ToString(),
                        DayOff = DayOfWeek.Sunday.ToString(),
                        StartHoliday = DateTime.Today.ToString(),
                        EndHoliday = DateTime.Today.AddDays(7).ToString(),
                        Role = "Supervisor"
                    }
                );
                db.SaveChanges();
            }

            builder.Configure(app =>
            {
                app.UseDeveloperExceptionPage();
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "Test", options => { }
            );
        });
    }
}