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
        await context.InitProductsAndCategories();
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

    private static async Task InitProductsAndCategories(this ApplicationDbContext context) {
        if (context.Categories.Any() || context.Products.Any())
            return;

        var categories = new List<Category> {
            new() { Name = "گل‌های زینتی" },
            new() { Name = "بذرها" },
            new() { Name = "خوراکی‌ها" },
            new() { Name = "ابزارهای باغبانی" }
        };
        context.Categories.AddRange(categories);

        context.Products.AddRange(
            new Product {
                Name          = "گل رز قرمز",
                Description   = "گل رز قرمز با گلبرگ‌های زیبا و خوشبو که برای هدیه دادن عالی است.",
                Category      = new List<ProductCategory> { new() { Category = categories[0] } },
                Price         = 25000,
                StockQuantity = 50,
                Image         = "rose.jpg",
                ProductWeight = 0.5,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "گل لاله زرد",
                Description   = "گل لاله زرد با رنگی خوش‌آمیز که در باغ‌ها به عنوان تزیین استفاده می‌شود.",
                Category      = new List<ProductCategory> { new() { Category = categories[0] } },
                Price         = 18000,
                StockQuantity = 40,
                Image         = "tulip.jpg",
                ProductWeight = 0.4,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "گل بنفشه",
                Description   = "گل بنفشه با رنگ‌های زیبا و خوشبو، یک گزینه عالی برای باغ‌ها و گلدان‌ها.",
                Category      = new List<ProductCategory> { new() { Category = categories[0] } },
                Price         = 22000,
                StockQuantity = 35,
                Image         = "lilac.jpg",
                ProductWeight = 0.6,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "گل زنبق سفید",
                Description   = "گل زنبق سفید با زیبایی ساده‌اش به عنوان نماد صفا و پاکی مورد استفاده قرار می‌گیرد.",
                Category      = new List<ProductCategory> { new() { Category = categories[0] } },
                Price         = 30000,
                StockQuantity = 25,
                Image         = "lily.jpg",
                ProductWeight = 0.7,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "گل داوودی",
                Description   = "گل داوودی با رنگ‌های جذاب و زیبا که به سرعت در فصل بهار می‌رشد.",
                Category      = new List<ProductCategory> { new() { Category = categories[0] } },
                Price         = 15000,
                StockQuantity = 60,
                Image         = "daffodil.jpg",
                ProductWeight = 0.3,
                IsAvailable   = true,
                IsFeatured    = true
            },

            // Category: بذرها
            new Product {
                Name          = "بذر گل مریم",
                Description   = "بذر گل مریم مناسب برای کاشت در باغ و تزیین فضای باز.",
                Category      = new List<ProductCategory> { new() { Category = categories[1] } },
                Price         = 5000,
                StockQuantity = 100,
                Image         = "marigold.jpg",
                ProductWeight = 0.1,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "بذر گلابی",
                Description   = "بذر گلابی برای کاشت در باغ و تولید میوه‌های خوشمزه.",
                Category      = new List<ProductCategory> { new() { Category = categories[1] } },
                Price         = 8000,
                StockQuantity = 80,
                Image         = "pear.jpg",
                ProductWeight = 0.2,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "بذر گل سرخ",
                Description   = "بذر گل سرخ برای تزیین گلدان‌ها و باغچه‌ها.",
                Category      = new List<ProductCategory> { new() { Category = categories[1] } },
                Price         = 4000,
                StockQuantity = 120,
                Image         = "red-flower.jpg",
                ProductWeight = 0.05,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "بذر گل مغرنسیا",
                Description   = "بذر گل مغرنسیا با رنگ‌های شاد و متنوع برای تزیین فضای باز.",
                Category      = new List<ProductCategory> { new() { Category = categories[1] } },
                Price         = 6500,
                StockQuantity = 90,
                Image         = "magnolia.jpg",
                ProductWeight = 0.12,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "بذر گل مو",
                Description   = "بذر گل مو برای کاشت در گلدان‌ها و تزیین محیط‌های داخلی.",
                Category      = new List<ProductCategory> { new() { Category = categories[1] } },
                Price         = 3500,
                StockQuantity = 150,
                Image         = "maidenhair-fern.jpg",
                ProductWeight = 0.08,
                IsAvailable   = true,
                IsFeatured    = true
            },

            // Category: خوراکی‌ها
            new Product {
                Name          = "سبزیجات آلی",
                Description   = "مجموعه‌ای از سبزیجات تازه و آلی برای مصرف خانگی.",
                Category      = new List<ProductCategory> { new() { Category = categories[2] } },
                Price         = 15000,
                StockQuantity = 30,
                Image         = "organic-vegetables.jpg",
                ProductWeight = 1.0,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "میوه‌های تازه",
                Description   = "میوه‌های تازه و تر، از باغ‌های ما به سفر خود ببرید.",
                Category      = new List<ProductCategory> { new() { Category = categories[2] } },
                Price         = 18000,
                StockQuantity = 25,
                Image         = "fresh-fruits.jpg",
                ProductWeight = 1.2,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "عسل طبیعی",
                Description   = "عسل طبیعی و خالص از زنبورستان ما.",
                Category      = new List<ProductCategory> { new() { Category = categories[2] } },
                Price         = 25000,
                StockQuantity = 40,
                Image         = "honey.jpg",
                ProductWeight = 0.5,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "شیرینی‌های سنتی",
                Description   = "شیرینی‌های سنتی و خوشمزه برای لذت بردن از زندگی.",
                Category      = new List<ProductCategory> { new() { Category = categories[2] } },
                Price         = 12000,
                StockQuantity = 50,
                Image         = "traditional-sweets.jpg",
                ProductWeight = 0.8,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "دانه‌های خشک",
                Description   = "مجموعه‌ای از دانه‌های خشک برای مصرف خانگی یا در سفرهای خارجی.",
                Category      = new List<ProductCategory> { new() { Category = categories[2] } },
                Price         = 10000,
                StockQuantity = 60,
                Image         = "nuts.jpg",
                ProductWeight = 0.4,
                IsAvailable   = true,
                IsFeatured    = false
            },

            // Category: ابزارهای باغبانی
            new Product {
                Name          = "ابزار باغبانی کامل",
                Description   = "مجموعه‌ای از ابزارهای باغبانی حرفه‌ای برای حیاط خود.",
                Category      = new List<ProductCategory> { new() { Category = categories[3] } },
                Price         = 75000,
                StockQuantity = 10,
                Image         = "gardening-tools.jpg",
                ProductWeight = 2.5,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "خرطومی آبیاری",
                Description   = "خرطومی آبیاری با عملکرد عالی برای نگهداری گیاهان شما.",
                Category      = new List<ProductCategory> { new() { Category = categories[3] } },
                Price         = 35000,
                StockQuantity = 15,
                Image         = "hose.jpg",
                ProductWeight = 1.2,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "بذراشکار",
                Description   = "بذراشکار برای کاشت دقیق بذرها در خاک.",
                Category      = new List<ProductCategory> { new() { Category = categories[3] } },
                Price         = 25000,
                StockQuantity = 20,
                Image         = "seeder.jpg",
                ProductWeight = 0.8,
                IsAvailable   = true,
                IsFeatured    = false
            },
            new Product {
                Name          = "پره‌زن بادی",
                Description   = "پره‌زن بادی برای جلوگیری از حمله حشرات به گیاهان شما.",
                Category      = new List<ProductCategory> { new() { Category = categories[3] } },
                Price         = 18000,
                StockQuantity = 30,
                Image         = "wind-spinner.jpg",
                ProductWeight = 0.3,
                IsAvailable   = true,
                IsFeatured    = true
            },
            new Product {
                Name          = "گلدان‌های چوبی",
                Description   = "گلدان‌های چوبی با طراحی زیبا برای نگهداری گیاهان شما.",
                Category      = new List<ProductCategory> { new() { Category = categories[3] } },
                Price         = 20000,
                StockQuantity = 25,
                Image         = "wooden-planters.jpg",
                ProductWeight = 0.6,
                IsAvailable   = true,
                IsFeatured    = false
            }
        );
        await context.SaveChangesAsync();
    }
}