using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Areas.Identity;

namespace Shop.Data;

public static class DbInitializer {
    public static async Task InitializeAsync(IServiceProvider serviceProvider) {
        await using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<ApplicationDbContext>>());

        await InitUsersAsync(serviceProvider);
    }

    private static async Task InitUsersAsync(IServiceProvider serviceProvider) {
        var adminId = await EnsureUser(serviceProvider, "admin@shop.com", "admin@shop.com");
        await EnsureRole(serviceProvider, adminId, Roles.Admin);
    }

    private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                 string testUserPw, string userName) {
        var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

        var user = await userManager!.FindByNameAsync(userName);
        if (user is null) {
            user = new IdentityUser {
                Email          = userName,
                UserName       = userName,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, testUserPw);
        }

        if (user is null)
            throw new Exception("The password is probably not strong enough!");

        return user.Id;
    }

    private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                         string uid, string role) {
        var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
        if (roleManager is null)
            throw new Exception("roleManager null");

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
        if (userManager is null)
            throw new Exception("userManager is null");

        var user = await userManager.FindByIdAsync(uid);
        if (user is null)
            throw new Exception($"The user with id ({uid}) is not exists.");

        var identityResult = await userManager.AddToRoleAsync(user, role);
        return identityResult;
    }
}