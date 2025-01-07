using Business_Logic_Layer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.DTO
{
    public class CategoryRequestDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Category name must be between 1 and 50 characters.", MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Description must be 200 characters or less.")]
        public string Description { get; set; }

        public Guid? ParentCategoryId { get; set; }

        [Required]
        public CategoryStatus Status { get; set; }
    }
}
