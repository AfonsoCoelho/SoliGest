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

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Fábrica personalizada de aplicação para testes de integração.
    /// Esta classe é usada para configurar a aplicação para execução de testes, incluindo configuração do banco de dados em memória e autenticação personalizada.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        /// <summary>
        /// Configura os serviços do host da aplicação para os testes de integração.
        /// Substitui a configuração padrão do banco de dados para usar um banco de dados em memória e inicializa dados de teste.
        /// </summary>
        /// <param name="builder">O construtor do host da aplicação.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove a configuração existente do DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SoliGestServerContext>)
                );
                if (descriptor != null)
                    services.Remove(descriptor);

                // Configura o DbContext para usar um banco de dados em memória
                services.AddDbContext<SoliGestServerContext>(opts =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    var uniqueName = $"TestDb_{Guid.NewGuid()}";
                    opts.UseInMemoryDatabase(uniqueName)
                        .UseInternalServiceProvider(serviceProvider);
                });

                // Configura a página de erro de desenvolvedor
                builder.Configure(app =>
                {
                    app.UseDeveloperExceptionPage();
                });

                // Semeia dados no banco de dados de teste
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<SoliGestServerContext>();
                    db.Database.EnsureCreated();

                    // Adiciona dois usuários para os testes
                    db.Users.AddRange(
                        new User
                        {
                            Id = "test-user",
                            Name = "Test User1",
                            Address1 = "Rua Principal, 123",
                            Address2 = "Apto 45",
                            BirthDate = new DateTime(1990, 1, 1).ToString(),
                            DayOff = DayOfWeek.Sunday.ToString(),
                            StartHoliday = DateTime.Today.ToString(),
                            EndHoliday = DateTime.Today.AddDays(7).ToString(),
                            Role = "Supervisor"
                        },
                        new User
                        {
                            Id = "1",
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

                // Configura novamente a página de erro de desenvolvedor
                builder.Configure(app =>
                {
                    app.UseDeveloperExceptionPage();
                });

                // Configura a autenticação personalizada para testes
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
}
