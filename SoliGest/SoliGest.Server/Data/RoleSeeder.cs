using Microsoft.AspNetCore.Identity;

namespace SoliGest.Server.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager, ILogger<Program> logger)
        {
            string[] roleNames = { "Supervisor", "Administrative", "Technician" };
            foreach (var roleName in roleNames)
            {
                try
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("RoleSeeder: " + ex);
                    logger.LogError("Role Seeder: " + ex);
                }
            }
        }
    }
}
