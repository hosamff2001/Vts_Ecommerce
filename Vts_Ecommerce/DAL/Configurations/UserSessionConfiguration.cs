using System.Data.Entity.ModelConfiguration;
using Vts_Ecommerce.Models;

public class UserSessionConfiguration : EntityTypeConfiguration<UserSession>
{
    public UserSessionConfiguration()
    {
        HasKey(us => us.Id);

        Property(us => us.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        Property(us => us.DeviceInfo)
            .HasMaxLength(500);

        HasIndex(us => us.SessionId).IsUnique();
    }
}
