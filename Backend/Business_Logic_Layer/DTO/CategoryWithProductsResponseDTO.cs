using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.DTO
{
    public class CategoryWithProductsResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<ProductResponseDTO> Products { get; set; } = new List<ProductResponseDTO>();
        public List<CategoryWithProductsResponseDTO> Subcategories { get; set; } = new List<CategoryWithProductsResponseDTO>();
    }

    
}
