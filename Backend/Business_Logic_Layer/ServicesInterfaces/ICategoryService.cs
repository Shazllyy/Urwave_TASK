using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business_Logic_Layer.ServicesInterfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid categoryId);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid categoryId, Guid? newCategoryId = null);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<CategoryResponseDTO?> GetSubcategoriesAsync(Guid parentCategoryId);



    }
}
