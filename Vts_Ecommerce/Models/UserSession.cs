using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vts_Ecommerce.Models
{
    /// <summary>
    /// UserSession entity for tracking active user sessions
    /// Used for single-device login enforcement
    /// Used ONLY for Entity Framework Code First schema generation
    /// </summary>
    [Table("UserSessions")]
    public class UserSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string SessionId { get; set; }

        [StringLength(500)]
        public string? DeviceInfo { get; set; }

        [Required]
        public DateTime LoginTime { get; set; } = DateTime.Now;

        [Required]
        public DateTime LastActivityTime { get; set; } = DateTime.Now;

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}

