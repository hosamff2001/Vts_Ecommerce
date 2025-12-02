using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class ProductConfiguration : EntityTypeConfiguration<Product>
{
    public ProductConfiguration()
    {
        HasKey(p => p.Id);

        Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        Property(p => p.Description)
            .HasMaxLength(1000);

        Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        Property(p => p.StockQuantity)
            .IsRequired();

        Property(p => p.IsActive)
            .IsRequired();

        HasMany(p => p.InvoiceItems)
            .WithRequired(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .WillCascadeOnDelete(false);
    }
}
