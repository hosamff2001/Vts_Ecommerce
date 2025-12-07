using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vts_Ecommerce.Models
{
    /// <summary>
    /// Product entity for inventory management
    /// Used ONLY for Entity Framework Code First schema generation
    /// </summary>
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public decimal CostPrice { get; set; } = 0;

        [Required]
        public decimal SellingPrice { get; set; } = 0;

        [Required]
        public int StockQuantity { get; set; } = 0;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<SalesInvoiceItem>? InvoiceItems { get; set; }
        public decimal Price { get; set; }
    }
}

