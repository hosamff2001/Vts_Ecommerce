using Vts_Ecommerce.DAL;
using Vts_Ecommerce.DAL.DataSeeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support (MUST be registered before builder.Build())
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3); // 3-hour session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the application
var app = builder.Build();

// Initialize AdoHelper with connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection connection string not found in appsettings.json");
}
AdoHelper.Initialize(connectionString);

// Seed database with dummy data on startup
DatabaseSeeder.SeedData();





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use session middleware (MUST be called after UseRouting and before UseAuthorization)
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();