using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL
{
    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            // Disable database initialization
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove pluralization convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserSessionConfiguration());
            modelBuilder.Configurations.Add(new CategoryConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration());
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new SalesInvoiceConfiguration());
            modelBuilder.Configurations.Add(new SalesInvoiceItemConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}

