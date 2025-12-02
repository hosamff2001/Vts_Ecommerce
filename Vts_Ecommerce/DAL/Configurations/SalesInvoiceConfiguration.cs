using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class SalesInvoiceConfiguration : EntityTypeConfiguration<SalesInvoice>
{
    public SalesInvoiceConfiguration()
    {
        HasKey(i => i.Id);

        Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        Property(i => i.SubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        Property(i => i.ItemDiscount)
            .HasPrecision(18, 2)
            .IsRequired();

        Property(i => i.InvoiceDiscount)
            .HasPrecision(18, 2)
            .IsRequired();

        Property(i => i.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        HasIndex(i => i.InvoiceNumber).IsUnique();

        HasMany(i => i.InvoiceItems)
            .WithRequired(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId)
            .WillCascadeOnDelete(true);
    }
}
