using Business_Logic_Layer.Enums;
using Data_Access_Layer.ApplicationDbContext;
using Data_Access_Layer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbcontext _context;
        private readonly DbSet<Category> _dbSet;

        public CategoryRepository(ApplicationDbcontext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Category>();
        }

        public async Task<IEnumerable<Category>> GetByParentCategoryAsync(Guid? parentCategoryId)
        {
            return await _dbSet.Where(c => c.ParentCategoryId == parentCategoryId).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetByStatusAsync(CategoryStatus status)
        {
            return await _dbSet.Where(c => c.Status == status).ToListAsync();
        }

        public async Task<bool> CanDeleteCategoryAsync(Guid categoryId)
        {
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
            if (hasProducts) return false; // Cannot delete if products exist

            var hasSubCategories = await _dbSet.AnyAsync(c => c.ParentCategoryId == categoryId);
            if (hasSubCategories) return false; // Cannot delete if subcategories exist

            return true; // Category can be deleted
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _context.Category
                                 .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task DeleteCategoryAsync(Guid categoryId, Guid? newCategoryId = null)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category == null) throw new Exception("Category not found.");

            // If products exist and a new category is provided, move them
            if (newCategoryId.HasValue)
            {
                var productsToMove = await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
                foreach (var product in productsToMove)
                {
                    product.CategoryId = newCategoryId.Value;
                    _context.Products.Update(product);
                }
                await _context.SaveChangesAsync();
            }

            // Proceed with category deletion
            _dbSet.Remove(category);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Category>> GetSubcategoriesAsync(Guid parentCategoryId)
        {
            return await _context.Category
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .ToListAsync();
        }
    }
}
