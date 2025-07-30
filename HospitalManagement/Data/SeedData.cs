using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using HospitalManagement.Models;
using HospitalManagement.Data;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;

public static class SeedData
{
    public static async Task CreateRolesAndDefaultUser(this IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserBaseModel>>();
            var _context = scope.ServiceProvider.GetRequiredService<HospitalManagementDbContext>();

            string[] roleNames = { "System Administrator",
                "Doctor", 
                "Paramedic",
                "Pharmacist",
                "Kitchen Staff",
                "Delivery Personnel",
                "Receptionist"
            };


            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var personnelAdmin = await userManager.FindByEmailAsync("admin@gmail.com");
            if (personnelAdmin == null)
            {
                var defaultUser = new SystemAdministrator
                {
                    FirstName = "Hospital",
                    LastName = "Admin",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "0665242536",
                    DateOfBirth = DateTime.Now,
                    ProfilePicture = "UploadedFiles/default_profile_image.jpg",
                    EmailConfirmed = true,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    CreatedBy = "default-user-id",
                    ModifiedBy = "default-user-id",
                    IsActive = true,
                    IsDeleted = false,
                    IsFirstTimeLogin = true,
                    IsSuspended = false
                };

                var result = await userManager.CreateAsync(defaultUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "System Administrator");
                }
            }


            await _context.SaveChangesAsync();
        }
    }
}
