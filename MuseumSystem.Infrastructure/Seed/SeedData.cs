using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MuseumSystem.Domain.Enums;
using MuseumSystem.Infrastructure.DatabaseSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Infrastructure.Seed
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Implementation for seeding data goes here
            using var scope = serviceProvider.CreateScope();
            var seedService = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var role = await seedService.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            if (role == null)
            {
                role = new Domain.Entities.Role
                {
                    Name = "SuperAdmin"
                };
                await seedService.Roles.AddAsync(role);
                await seedService.SaveChangesAsync();
            }

            var email = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL");
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("SUPERADMIN_EMAIL is not set in environment variables.");
                return;
            }
            var user = await seedService.Accounts.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new Domain.Entities.Account
                {
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD")),
                    FullName = "System Super Admin",
                    RoleId = role.Id,
                    Status = EnumStatus.Active,
                };
                await seedService.Accounts.AddAsync(user);
                await seedService.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Super admin account already exists.");
            }
        }
    }
}
