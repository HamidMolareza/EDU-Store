using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Areas.Identity;
using Shop.Models;

namespace Shop.Data;

public static class DbInitializer {
    public static async Task InitializeAsync(IServiceProvider serviceProvider) {
        await using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<ApplicationDbContext>>());

        await InitUsersAsync(serviceProvider);
        await context.InitContactUsMessages();
    }

    private static async Task InitUsersAsync(IServiceProvider serviceProvider) {
        var adminId = await EnsureUser(serviceProvider, "admin@shop.com", "admin@shop.com");
        await EnsureRole(serviceProvider, adminId, Roles.Admin);

        await EnsureUser(serviceProvider, "user@user.com", "user@user.com");
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

    private static async Task InitContactUsMessages(this ApplicationDbContext context) {
        if (context.Messages.Any()) {
            return;
        }

        context.Messages.AddRange(
            new ContactUsMessage {
                Name    = "محمد حسینی",
                Email   = "mohsen@example.com",
                Title   = "پیشنهادات بهبودی",
                Message = "سلام، من چند پیشنهاد برای بهبود سایت دارم. آیا می‌توانم با شما در مورد آنها صحبت کنم؟"
            },
            new ContactUsMessage {
                Name  = "سارا اکبری",
                Email = "sara@example.com",
                Title = "اطلاعات بیشتر",
                Message =
                    "سلام، می‌خواستم بیشتر در مورد محصولات شما و قیمت‌هاشان اطلاعات بگیرم. آیا می‌توانید راهنمایی کنید؟"
            },
            new ContactUsMessage {
                Name    = "علی رضایی",
                Email   = "ali@example.com",
                Title   = "مشکل در ورود به حساب کاربری",
                Message = "سلام، من نمی‌توانم به حساب کاربری‌ام وارد شوم. آیا می‌توانید کمک کنید؟"
            },
            new ContactUsMessage {
                Name    = "نگین میرزایی",
                Email   = "negin@example.com",
                Title   = "پیشنهاد همکاری",
                Message = "سلام، من علاقه دارم با شما همکاری کنم. آیا می‌توانیم در مورد این موضوع صحبت کنیم؟"
            },
            new ContactUsMessage {
                Name  = "رضا خوشنام",
                Email = "reza@example.com",
                Title = "نظر در مورد محصول جدید",
                Message =
                    "سلام، محصول جدید شما بسیار جذاب به نظر می‌آید. من نظرات خود را در مورد آن به اشتراک می‌گذارم."
            },
            new ContactUsMessage {
                Name    = "پریسا کاظمی",
                Email   = "parisa@example.com",
                Title   = "اشتراک خبرنامه",
                Message = "سلام، آیا می‌توانید من را در اشتراک خبرنامه خود قرار دهید؟"
            },
            new ContactUsMessage {
                Name    = "مهدی محمودی",
                Email   = "mehdi@example.com",
                Title   = "اشکال در پرداخت",
                Message = "سلام، من در هنگام پرداخت با مشکل مواجه شدم. لطفاً راهنمایی کنید."
            },
            new ContactUsMessage {
                Name    = "فاطمه صادقی",
                Email   = "fatemeh@example.com",
                Title   = "تقدیر و تشکر",
                Message = "سلام، من از خدمات شما بسیار راضی هستم. ممنون از تلاش‌های شما."
            },
            new ContactUsMessage {
                Name    = "محمدعلی رحیمی",
                Email   = "mohamadali@example.com",
                Title   = "سوال در مورد API",
                Message = "سلام، من سوالی در مورد استفاده از API شما دارم. آیا می‌توانید راهنمایی کنید؟"
            },
            new ContactUsMessage {
                Name  = "زهرا میری",
                Email = "zahra@example.com",
                Title = "انتقادات و پیشنهادات",
                Message =
                    "سلام، من چند انتقاد و پیشنهاد دارم که فکر می‌کنم می‌توانند به بهبود کیفیت خدمات شما کمک کنند."
            }
        );
        await context.SaveChangesAsync();
    }
}