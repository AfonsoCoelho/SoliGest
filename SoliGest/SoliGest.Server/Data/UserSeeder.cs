using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Classe responsável por popular a tabela de usuários no banco de dados.
    /// A classe verifica se os usuários já existem com base no e-mail e, caso contrário, cria usuários com dados de exemplo.
    /// </summary>
    public class UserSeeder
    {
        /// <summary>
        /// Método assíncrono responsável por semear a tabela de usuários com dados de exemplo.
        /// Verifica se o usuário com o e-mail especificado já existe no banco de dados, e caso contrário, cria um novo usuário.
        /// </summary>
        /// <param name="userManager">O gerenciador de usuários do ASP.NET Identity, usado para manipulação de usuários.</param>
        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // Verifica se o usuário com o e-mail 'soligestesa@gmail.com' já existe
            if (userManager.FindByEmailAsync("soligestesa@gmail.com").Result == null)
            {
                User user = new User
                {
                    Email = "soligestesa@gmail.com",
                    Name = "SoliGest Supervisor",
                    UserName = "soligestesa@gmail.com",
                    Address1 = "morada1",
                    Address2 = "morada2",
                    BirthDate = "2001-01-01",
                    PhoneNumber = "999999999",
                    Role = "Supervisor",
                    DayOff = "Sábado",
                    StartHoliday = "2025-06-01",
                    EndHoliday = "2025-07-01",
                };

                // Cria o usuário e adiciona à função 'Supervisor'
                await userManager.CreateAsync(user, "Admin1!");
                await userManager.AddToRoleAsync(user, "Supervisor");
            }

            // Verifica e cria o usuário com o e-mail 'administrative@mail.com'
            if (userManager.FindByEmailAsync("administrative@mail.com").Result == null)
            {
                User user = new User
                {
                    Email = "administrative@mail.com",
                    Name = "SoliGest Administrative",
                    UserName = "administrative@mail.com",
                    Address1 = "morada1",
                    Address2 = "morada2",
                    BirthDate = "2001-01-01",
                    PhoneNumber = "999999999",
                    Role = "Administrativo",
                    DayOff = "Sábado",
                    StartHoliday = "2025-06-01",
                    EndHoliday = "2025-07-01",
                };

                await userManager.CreateAsync(user, "Admin1!");
                await userManager.AddToRoleAsync(user, "Administrative");
            }

            // Verifica e cria o usuário com o e-mail 'technician@mail.com'
            if (userManager.FindByEmailAsync("technician@mail.com").Result == null)
            {
                User user = new User
                {
                    Email = "technician@mail.com",
                    Name = "SoliGest Technician",
                    UserName = "technician@mail.com",
                    Address1 = "morada1",
                    Address2 = "morada2",
                    BirthDate = "2001-01-01",
                    PhoneNumber = "999999999",
                    Role = "Técnico",
                    DayOff = "Sábado",
                    StartHoliday = "2025-06-01",
                    EndHoliday = "2025-07-01",
                };

                await userManager.CreateAsync(user, "Tech1!");
                await userManager.AddToRoleAsync(user, "Technician");
            }

            // Verifica e cria o usuário com o e-mail 'technoya1@mail.com'
            if (userManager.FindByEmailAsync("technoya1@mail.com").Result == null)
            {
                User user = new User
                {
                    Email = "technoya1@mail.com",
                    Name = "SoliGest Tech n1",
                    UserName = "technoya1@mail.com",
                    Address1 = "morada111",
                    Address2 = "morada2222",
                    BirthDate = "2001-01-01",
                    PhoneNumber = "999999999",
                    Role = "Técnico",
                    DayOff = "Domingo",
                    StartHoliday = "2025-09-01",
                    EndHoliday = "2025-10-01",
                };

                await userManager.CreateAsync(user, "Tech2!");
                await userManager.AddToRoleAsync(user, "Technician");
            }

            // Verifica e cria o usuário com o e-mail 'chickenjockey@mail.com'
            if (userManager.FindByEmailAsync("chickenjockey@mail.com").Result == null)
            {
                User user = new User
                {
                    Email = "chickenjockey@mail.com",
                    Name = "Steve IAm",
                    UserName = "chickenjockey@mail.com",
                    Address1 = "theoverworld",
                    Address2 = "diamondstreet",
                    BirthDate = "2001-01-01",
                    PhoneNumber = "999999999",
                    Role = "Técnico",
                    DayOff = "Terça-feira",
                    StartHoliday = "2025-09-09",
                    EndHoliday = "2025-10-10",
                };

                await userManager.CreateAsync(user, "Tech3!");
                await userManager.AddToRoleAsync(user, "Technician");
            }
        }
    }
}
