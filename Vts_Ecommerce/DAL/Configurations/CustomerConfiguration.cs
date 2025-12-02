using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class CustomerConfiguration : EntityTypeConfiguration<Customer>
{
    public CustomerConfiguration()
    {
        HasKey(c => c.Id);

        Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        Property(c => c.Email)
            .HasMaxLength(100);

        Property(c => c.Phone)
            .HasMaxLength(20);

        Property(c => c.Address)
            .HasMaxLength(500);

        Property(c => c.IsActive)
            .IsRequired();

        HasMany(c => c.Invoices)
            .WithRequired(i => i.Customer)
            .HasForeignKey(i => i.CustomerId)
            .WillCascadeOnDelete(false);
    }
}
