using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vts_Ecommerce.Models
{
    /// <summary>
    /// User entity for authentication and authorization
    /// Used ONLY for Entity Framework Code First schema generation
    /// </summary>
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; } // Encrypted password

        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<UserSession> UserSessions { get; set; }
        public virtual ICollection<SalesInvoice> CreatedInvoices { get; set; }
    }
}

