using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class CategoryConfiguration : EntityTypeConfiguration<Category>
{
    public CategoryConfiguration()
    {
        HasKey(c => c.Id);

        Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        Property(c => c.Description)
            .HasMaxLength(500);

        Property(c => c.IsActive)
            .IsRequired();

        HasMany(c => c.Products)
            .WithRequired(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .WillCascadeOnDelete(false);
    }
}
