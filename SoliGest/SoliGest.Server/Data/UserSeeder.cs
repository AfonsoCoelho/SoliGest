using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;

namespace SoliGest.Server.Data
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
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

                await userManager.CreateAsync(user, "Admin1!");
                await userManager.AddToRoleAsync(user, "Supervisor");
            }

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
