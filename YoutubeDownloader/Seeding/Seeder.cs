using System.Net;
using Microsoft.AspNetCore.Identity;
using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.Seeding;

public class Seeder
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    
    private enum UserRoles
    {
        Admin,
        Regular
    }

    public Seeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<bool> SeedAsync()
    {
        foreach (var role in Enum.GetValues(typeof(UserRoles)))
        {
            await _roleManager.CreateAsync(new IdentityRole(role.ToString()));

        }
        
        var adminUser = new User
        {
            Id = "AdminUser",
            UserName = "Admin",
            Email = "admin@admin.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.UtcNow,
            ProfilePicSource = "xd"
        };
        if ((await _userManager.FindByIdAsync("AdminUser")) == null)
        {
            await _userManager.CreateAsync(adminUser, "Abc@123");
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    
        return true;
    }
}