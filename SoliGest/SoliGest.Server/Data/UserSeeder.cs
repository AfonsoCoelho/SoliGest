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
                    Name = "SoliGest Admin",
                    UserName = "soligestesa@gmail.com"
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
                    UserName = "administrative@mail.com"
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
                    UserName = "technician@mail.com"
                };

                await userManager.CreateAsync(user, "Tech1!");
                await userManager.AddToRoleAsync(user, "Technician");
            }
        }
    }
}
