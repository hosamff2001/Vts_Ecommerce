using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class UserConfiguration : EntityTypeConfiguration<User>
{
    public UserConfiguration()
    {
        HasKey(u => u.Id);

        Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        Property(u => u.Email)
            .HasMaxLength(100);

        Property(u => u.IsActive)
            .IsRequired();

        HasMany(u => u.UserSessions)
            .WithRequired(us => us.User)
            .HasForeignKey(us => us.UserId)
            .WillCascadeOnDelete(false);

        HasMany(u => u.CreatedInvoices)
            .WithRequired(s => s.CreatedByUser)
            .HasForeignKey(s => s.CreatedBy)
            .WillCascadeOnDelete(false);
    }
}
