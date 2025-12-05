using Vts_Ecommerce.DAL;
using Vts_Ecommerce.DAL.DataSeeding;
using Vts_Ecommerce.Middleware;

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

// Test database connection
try
{
    DatabaseConnectionTest.TestConnection(connectionString);
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Database connection test failed: {ex.Message}");
    Console.WriteLine("⚠ Please ensure the database is created and running.");
}

// Ensure database schema has expected columns added by recent code changes
try
{
    // Add missing columns if needed (CostPrice, SellingPrice on Products and Notes on Customers)
    var costPriceExists = (int)AdoHelper.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Products' AND COLUMN_NAME='CostPrice'");
    if (costPriceExists == 0)
    {
        Console.WriteLine("Adding missing column 'CostPrice' to Products table...");
        AdoHelper.ExecuteNonQuery("ALTER TABLE Products ADD CostPrice DECIMAL(18,2) NOT NULL CONSTRAINT DF_Products_CostPrice DEFAULT 0");
    }

    var sellingPriceExists = (int)AdoHelper.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Products' AND COLUMN_NAME='SellingPrice'");
    if (sellingPriceExists == 0)
    {
        Console.WriteLine("Adding missing column 'SellingPrice' to Products table and migrating Price -> SellingPrice if present...");
        AdoHelper.ExecuteNonQuery("ALTER TABLE Products ADD SellingPrice DECIMAL(18,2) NOT NULL CONSTRAINT DF_Products_SellingPrice DEFAULT 0");
        var priceExists = (int)AdoHelper.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Products' AND COLUMN_NAME='Price'");
        if (priceExists > 0)
        {
            // migrate existing Price data into SellingPrice
            AdoHelper.ExecuteNonQuery("UPDATE Products SET SellingPrice = Price WHERE SellingPrice = 0");
        }
    }

    var notesExists = (int)AdoHelper.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Customers' AND COLUMN_NAME='Notes'");
    if (notesExists == 0)
    {
        Console.WriteLine("Adding missing column 'Notes' to Customers table...");
        AdoHelper.ExecuteNonQuery("ALTER TABLE Customers ADD Notes NVARCHAR(1000) NULL");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Schema adjustment failed: {ex.Message}");
    // continue startup; controllers will still show errors if deeper schema mismatches exist
}

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

// Use session validation middleware to check session validity on each request
app.UseMiddleware<SessionValidationMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();