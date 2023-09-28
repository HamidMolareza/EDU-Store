using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Areas.Identity;
using Store.Models;

namespace Store.Data;

public static class DbInitializer {
    public static async Task InitializeAsync(IServiceProvider serviceProvider) {
        await using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<ApplicationDbContext>>());

        await InitUsersAsync(serviceProvider);
        await context.InitContactUsMessages();
        await context.InitProductsAndCategories();
    }

    private static async Task InitUsersAsync(IServiceProvider serviceProvider) {
        var adminId = await EnsureUser(serviceProvider, "admin@store.com", "admin@store.com");
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

    private static async Task InitProductsAndCategories(this ApplicationDbContext context) {
        if (context.Categories.Any() || context.Products.Any())
            return;

        var categories = new List<Category> {
            new() { Name = "گل‌های زینتی" },
            new() { Name = "بذرها" },
            new() { Name = "خوراکی‌ها" },
            new() { Name = "ابزارهای باغبانی" },
            new() { Name = "ویژه" },
        };
        context.Categories.AddRange(categories);

        context.Products.AddRange(
            new Product {
                Name        = "گل رز قرمز",
                Description = "گل رز قرمز با گلبرگ‌های زیبا و خوشبو که برای هدیه دادن عالی است.",
                ProductCategories = new List<ProductCategory>
                    { new() { Category = categories[0] }, new() { Category = categories[4] } },
                Price         = 25000,
                StockQuantity = 50,
                Image         = "/img/products/rose.jpg",
                ProductWeight = 0.5,
            },
            new Product {
                Name              = "گل لاله زرد",
                Description       = "گل لاله زرد با رنگی خوش‌آمیز که در باغ‌ها به عنوان تزیین استفاده می‌شود.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                Price             = 18000,
                StockQuantity     = 9,
                Image             = "/img/products/tulip.jpg",
                ProductWeight     = 0.4,
            },
            new Product {
                Name              = "گل بنفشه",
                Description       = "گل بنفشه با رنگ‌های زیبا و خوشبو، یک گزینه عالی برای باغ‌ها و گلدان‌ها.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                Price             = 22000,
                StockQuantity     = 35,
                Image             = "/img/products/lilac.jpg",
                ProductWeight     = 0.6
            },
            new Product {
                Name = "گل زنبق سفید",
                Description = "گل زنبق سفید با زیبایی ساده‌اش به عنوان نماد صفا و پاکی مورد استفاده قرار می‌گیرد.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                Price = 30000,
                StockQuantity = 25,
                Image = "/img/products/lily.jpg",
                ProductWeight = 0.7,
            },
            new Product {
                Name        = "گل داوودی",
                Description = "گل داوودی با رنگ‌های جذاب و زیبا که به سرعت در فصل بهار می‌رشد.",
                ProductCategories = new List<ProductCategory>
                    { new() { Category = categories[0] }, new() { Category = categories[4] } },
                Price         = 15000,
                StockQuantity = 60,
                Image         = "/img/products/daffodil.jpg",
                ProductWeight = 0.3,
            },

            // Category: بذرها
            new Product {
                Name              = "بذر گل مریم",
                Description       = "بذر گل مریم مناسب برای کاشت در باغ و تزیین فضای باز.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                Price             = 5000,
                StockQuantity     = 100,
                Image             = "/img/products/marigold.jpg",
                ProductWeight     = 0.1,
            },
            new Product {
                Name              = "بذر گلابی",
                Description       = "بذر گلابی برای کاشت در باغ و تولید میوه‌های خوشمزه.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                Price             = 8000,
                StockQuantity     = 8,
                Image             = "/img/products/pear.jpg",
                ProductWeight     = 0.2
            },
            new Product {
                Name              = "بذر گل مغرنسیا",
                Description       = "بذر گل مغرنسیا با رنگ‌های شاد و متنوع برای تزیین فضای باز.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                Price             = 6500,
                StockQuantity     = 90,
                Image             = "/img/products/magnolia.jpg",
                ProductWeight     = 0.12,
            },
            new Product {
                Name              = "بذر گل مو",
                Description       = "بذر گل مو برای کاشت در گلدان‌ها و تزیین محیط‌های داخلی.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                Price             = 3500,
                StockQuantity     = 150,
                Image             = "/img/products/maidenhair-fern.jpg",
                ProductWeight     = 0.08,
            },

            // Category: خوراکی‌ها
            new Product {
                Name              = "سبزیجات آلی",
                Description       = "مجموعه‌ای از سبزیجات تازه و آلی برای مصرف خانگی.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[2] } },
                Price             = 15000,
                StockQuantity     = 30,
                Image             = "/img/products/organic-vegetables.jpg",
                ProductWeight     = 1.0
            },
            new Product {
                Name              = "میوه‌های تازه",
                Description       = "میوه‌های تازه و تر، از باغ‌های ما به سفر خود ببرید.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[2] } },
                Price             = 18000,
                StockQuantity     = 3,
                Image             = "/img/products/fresh-fruits.jpg",
                ProductWeight     = 1.2
            },
            new Product {
                Name        = "عسل طبیعی",
                Description = "عسل طبیعی و خالص از زنبورستان ما.",
                ProductCategories = new List<ProductCategory>
                    { new() { Category = categories[2] }, new() { Category = categories[4] } },
                Price         = 25000,
                StockQuantity = 40,
                Image         = "/img/products/honey.png",
                ProductWeight = 0.5,
            },
            new Product {
                Name              = "آجیل",
                Description       = "مجموعه‌ای از آجیل برای مصرف خانگی یا در سفرهای خارجی.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[2] } },
                Price             = 10000,
                StockQuantity     = 60,
                Image             = "/img/products/nuts.jpg",
                ProductWeight     = 0.4
            },

            // Category: ابزارهای باغبانی
            new Product {
                Name        = "ابزار باغبانی کامل",
                Description = "مجموعه‌ای از ابزارهای باغبانی حرفه‌ای برای حیاط خود.",
                ProductCategories = new List<ProductCategory>
                    { new() { Category = categories[3] }, new() { Category = categories[4] } },
                Price         = 75000,
                StockQuantity = 6,
                Image         = "/img/products/gardening-tools.jpg",
                ProductWeight = 2.5,
            },
            new Product {
                Name              = "خرطومی آبیاری",
                Description       = "خرطومی آبیاری با عملکرد عالی برای نگهداری گیاهان شما.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[3] } },
                Price             = 35000,
                StockQuantity     = 15,
                Image             = "/img/products/hose.jpg",
                ProductWeight     = 1.2
            },
            new Product {
                Name              = "بذراشکار",
                Description       = "بذراشکار برای کاشت دقیق بذرها در خاک.",
                ProductCategories = new List<ProductCategory> { new() { Category = categories[3] } },
                Price             = 25000,
                StockQuantity     = 20,
                Image             = "/img/products/seeder.jpg",
                ProductWeight     = 0.8
            },
            new Product {
                Name        = "گلدان‌های چوبی",
                Description = "گلدان‌های چوبی با طراحی زیبا برای نگهداری گیاهان شما.",
                ProductCategories = new List<ProductCategory>
                    { new() { Category = categories[3] }, new() { Category = categories[4] } },
                Price         = 20000,
                StockQuantity = 25,
                Image         = "/img/products/wooden-planters.jpg",
                ProductWeight = 0.6
            }
        );
        await context.SaveChangesAsync();
    }
}