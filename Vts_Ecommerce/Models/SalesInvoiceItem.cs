using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vts_Ecommerce.Models
{
    /// <summary>
    /// SalesInvoiceItem entity for invoice line items
    /// Used ONLY for Entity Framework Code First schema generation
    /// </summary>
    [Table("SalesInvoiceItems")]
    public class SalesInvoiceItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal ItemDiscount { get; set; } = 0; // Per-item discount

        [Required]
        public decimal LineTotal { get; set; } // (Quantity * UnitPrice) - ItemDiscount

        // Navigation properties
        [ForeignKey("InvoiceId")]
        public virtual SalesInvoice Invoice { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}

