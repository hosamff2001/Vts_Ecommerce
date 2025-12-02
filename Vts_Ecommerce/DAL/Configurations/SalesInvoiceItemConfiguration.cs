using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class SalesInvoiceItemConfiguration : EntityTypeConfiguration<SalesInvoiceItem>
{
    public SalesInvoiceItemConfiguration()
    {
        HasKey(i => i.Id);

        Property(i => i.Quantity)
            .IsRequired();

        Property(i => i.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        Property(i => i.ItemDiscount)
            .HasPrecision(18, 2)
            .IsRequired();

        Property(i => i.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
