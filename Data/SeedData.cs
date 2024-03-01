using System;
using System.Linq;
using System.Threading.Tasks;
using Gravitas.Monitoring.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gravitas.Monitoring.Data
{
    public static class SeedData
    {
        private const string AdminEmail = "admin@admin.com";
        
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                SeedDB(context, AdminEmail);
                await AssignRoles(serviceProvider, AdminEmail, new []{ UserRoles.Admin, UserRoles.User, UserRoles.Modifier });
            }
        }

        private static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Users.Any(x => x.Email == adminID))
            {
                return;   // DB has been seeded
            }

            context.Roles.Add(new IdentityRole()
            {
                Name = UserRoles.Admin,
                NormalizedName = UserRoles.Admin.ToUpper()
            });
            context.Roles.Add(new IdentityRole()
            {
                Name = UserRoles.User,
                NormalizedName = UserRoles.User.ToUpper()
            });
            context.Roles.Add(new IdentityRole()
            {
                Name = UserRoles.Modifier,
                NormalizedName = UserRoles.Modifier.ToUpper()
            });
            context.SaveChanges();

            var user = new IdentityUser()
            {
                UserName = AdminEmail,
                NormalizedUserName = AdminEmail,
                Email = AdminEmail,
                NormalizedEmail = AdminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<IdentityUser>();
                var hashed = password.HashPassword(user,"Admin4321");
                user.PasswordHash = hashed;

                var userStore = new UserStore<IdentityUser>(context);
                var result = userStore.CreateAsync(user).Result;
            }
        }

        private static async Task<IdentityResult> AssignRoles(IServiceProvider serviceProvider, string email, string[] roles)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            var result = await userManager.AddToRolesAsync(user, roles);

            return result;
        }
    }
}