using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Extensions;

public static class StartupExtensions {
    public static IServiceCollection AddDatabase(this IServiceCollection services, bool isDevelopment,
                                                 ConfigurationManager configuration) {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");
        if (isDevelopment) {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(connectionString));
        }
        else {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services) {
        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.Configure<IdentityOptions>(options => {
            // Password settings.
            options.Password.RequireDigit           = false;
            options.Password.RequireLowercase       = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase       = false;
            options.Password.RequiredLength         = 3;
            options.Password.RequiredUniqueChars    = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(2);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers      = true;
        });

        return services;
    }
}