using Shop.Areas.Identity;
using Shop.Data;
using Shop.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var isDevelopment = true; //TODO: builder.Environment.IsDevelopment();
builder.Services.AddDatabase(isDevelopment, builder.Configuration)
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options => {
    options.AddPolicy(Policies.Admin,
        policy => policy.RequireRole(Roles.Admin));
});

builder.Services.AddIdentity();

builder.Services.AddRazorPages(options => { options.Conventions.AuthorizeAreaFolder("Admin", "/", Policies.Admin); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using var scope    = app.Services.CreateScope();
var       services = scope.ServiceProvider;
await DbInitializer.InitializeAsync(services);

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();