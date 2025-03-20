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
                    Name = "SoliGest Admin"
                };

                await userManager.CreateAsync(user, "Admin1!");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            if (userManager.FindByEmailAsync("administrative@mail.com").Result == null)
            {
                User user = new User
                {
                    Email = "administrative@gmail.com",
                    Name = "SoliGest Administrative"
                };

                await userManager.CreateAsync(user, "Admin1!");
                await userManager.AddToRoleAsync(user, "Administrative");
            }

            if (userManager.FindByEmailAsync("technician@mail.com").Result == null)
            {
                User user = new User
                {
                    Email = "technician@mail.com",
                    Name = "SoliGest Technician"
                };

                await userManager.CreateAsync(user, "Tech1!");
                await userManager.AddToRoleAsync(user, "Technician");
            }
        }
    }
}
