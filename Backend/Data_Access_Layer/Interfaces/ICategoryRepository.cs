using Business_Logic_Layer.Enums;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetByParentCategoryAsync(Guid? parentCategoryId);
        Task<IEnumerable<Category>> GetByStatusAsync(CategoryStatus status);
        Task<bool> CanDeleteCategoryAsync(Guid categoryId);
        Task DeleteCategoryAsync(Guid categoryId, Guid? newCategoryId = null);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<IEnumerable<Category>> GetSubcategoriesAsync(Guid parentCategoryId);


    }
}
