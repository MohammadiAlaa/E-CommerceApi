using Microsoft.AspNetCore.Identity;

namespace E_CommerceApi.Models
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User", "Driver", "Accountant" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var defaultAdminEmail = "admin@store.com";
            var defaultAdminPassword = "AdminPassword@123"; 

            if (await userManager.FindByEmailAsync(defaultAdminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "SuperAdmin",
                    Email = defaultAdminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, defaultAdminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
