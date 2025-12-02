using System.Data.Entity.Migrations;
using Vts_Ecommerce.Models;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Migrations
{
    /// <summary>
    /// Entity Framework Migration Configuration
    /// Used ONLY for database schema generation
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(DAL.ApplicationDbContext context)
        {
            // Seed initial data if needed
            // Example: Create default admin user
            // Note: Password should be encrypted using EncryptionService
            // This will be implemented in later phases
        }
    }
}

