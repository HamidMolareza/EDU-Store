using Shop.Data;
using Shop.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase(builder.Environment.IsDevelopment(), builder.Configuration)
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddIdentity();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();

    using var scope    = app.Services.CreateScope();
    var       services = scope.ServiceProvider;
    await DbInitializer.InitializeAsync(services);
}
else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();