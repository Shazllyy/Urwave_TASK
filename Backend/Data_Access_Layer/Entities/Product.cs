using Business_Logic_Layer.Enums;
using Data_Access_Layer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    public class Product : IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }  


        [Required]
        public ProductStatus Status { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be a non-negative value.")]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    }

}
