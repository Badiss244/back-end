using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Domain.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roleNames = { "Admin", "QualityM", "Auditor", "FactoryM" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole<Guid>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    };
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}