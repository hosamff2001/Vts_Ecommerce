using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vts_Ecommerce.Models
{
    /// <summary>
    /// SalesInvoice entity for invoice management
    /// Used ONLY for Entity Framework Code First schema generation
    /// </summary>
    [Table("SalesInvoices")]
    public class SalesInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        public decimal SubTotal { get; set; } = 0;

        [Required]
        public decimal ItemDiscount { get; set; } = 0; // Sum of all item-level discounts

        [Required]
        public decimal InvoiceDiscount { get; set; } = 0; // Invoice-level discount

        [Required]
        public decimal Total { get; set; } = 0;

        [Required]
        public int CreatedBy { get; set; } // UserId

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Convenience properties for controller
        [NotMapped]
        public decimal TotalAmount
        {
            get { return Total; }
            set { Total = value; }
        }

        [NotMapped]
        public decimal DiscountAmount
        {
            get { return InvoiceDiscount; }
            set { InvoiceDiscount = value; }
        }

        [NotMapped]
        public decimal TaxAmount { get; set; } = 0;

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        public virtual ICollection<SalesInvoiceItem>? InvoiceItems { get; set; }
    }
}

