using Azure;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.ServicesInterfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using Data_Access_Layer.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMemoryCache _cache;
        private const string CategoryCacheKey = "category_cache";

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _unitOfWork.Category.GetCategoryByNameAsync(name);

        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            // Attempt to get from cache
            if (!_cache.TryGetValue(CategoryCacheKey, out IEnumerable<Category> categories))
            {
                // If data is not in cache, retrieve it from the database
                categories = await _unitOfWork.Category.GetAllAsync();

                // Set cache with an expiration time of 10 minutes
                _cache.Set(CategoryCacheKey, categories, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                _logger.LogInformation("Cache miss: Retrieved categories from the database.");
            }
            else
            {
                _logger.LogInformation("Cache hit: Retrieved categories from the cache.");
            }

            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _unitOfWork.Category.GetByIdAsync(categoryId);
        }

        public async Task AddCategoryAsync(Category category)
        {
            _logger.LogInformation("Adding category: {CategoryName}", category.Name);

            // Add category to the database
            await _unitOfWork.Category.AddAsync(category);
            await _unitOfWork.SaveAsync();

            // Clear the cache after adding a new category
            _cache.Remove(CategoryCacheKey);

            // Log the addition of the category
            _logger.LogInformation("Category added: {CategoryName} at {CreatedDate}", category.Name, category.CreatedDate);

            // Example of logging some additional information (assuming you want to log other metadata instead of a response)
            _logger.LogInformation("Category with ID {CategoryId} was added successfully", category.Id);
        }


        public async Task UpdateCategoryAsync(Category category)
        {
            _logger.LogInformation("Updating category: {CategoryName}", category.Name);

            // Update category in the database
            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveAsync();

            // Clear the cache after updating the category
            _cache.Remove(CategoryCacheKey);

            // Log the update of the category
            _logger.LogInformation("Category updated: {CategoryName} at {UpdatedDate}", category.Name, category.UpdatedDate);
        }


        // Delete category and optionally move products to another category
        public async Task DeleteCategoryAsync(Guid categoryId, Guid? newCategoryId = null)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            // Check if the category can be deleted
            var canDelete = await _unitOfWork.Category.CanDeleteCategoryAsync(categoryId);
       

            _logger.LogInformation("Deleting category: {CategoryName}", category.Name);

            // If products exist and a new category is provided, move them
            if (newCategoryId.HasValue && !canDelete)
            {
                var productsToMove = await _unitOfWork.Product.GetByCategoryAsync(categoryId);
                foreach (var product in productsToMove)
                {
                    product.CategoryId = newCategoryId.Value;
                    _unitOfWork.Product.Update(product);
                }
                await _unitOfWork.SaveAsync();
                await _unitOfWork.Category.DeleteCategoryAsync(categoryId, newCategoryId);

            }
            else if(newCategoryId.HasValue && canDelete)
            {
                await _unitOfWork.Category.DeleteCategoryAsync(categoryId, newCategoryId);

            }
            else if (!canDelete)
            {
                throw new Exception("Category cannot be deleted because it has products or subcategories.");

            }
            else
            {
                await _unitOfWork.Category.DeleteCategoryAsync(categoryId, newCategoryId);

            }
            // Proceed with category deletion

            // Clear the cache after deleting a category
            _cache.Remove(CategoryCacheKey);

            // Log the deletion of the category
            _logger.LogInformation("Category deleted: {CategoryId}", categoryId);
        }
        public async Task<CategoryResponseDTO?> GetSubcategoriesAsync(Guid parentCategoryId)
        {
            // Get the main category using the parentCategoryId
            var mainCategory = await _unitOfWork.Category.GetByIdAsync(parentCategoryId);
            if (mainCategory == null)
            {
                return null; // Or handle it as you wish, maybe throw an exception or return a specific result.
            }

            // Get the subcategories for the given parent category ID
            var subcategories = await _unitOfWork.Category.GetSubcategoriesAsync(parentCategoryId);

            var subcategoryDTOs = new List<CategoryResponseDTO>();

            foreach (var subcategory in subcategories)
            {
                // Get the products for each subcategory
                var products = await _unitOfWork.Product.GetByCategoryAsync(subcategory.Id);

                // Map each subcategory and its products to the DTO
                var subcategoryDTO = new CategoryResponseDTO
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Description = subcategory.Description,
                    ParentCategoryId = subcategory.ParentCategoryId,
                    Status = subcategory.Status.ToString(),
                    CreatedDate = subcategory.CreatedDate,
                    UpdatedDate = subcategory.UpdatedDate,
                    Products = products.Select(p => new ProductResponseDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        status = p.Status.ToString(),
                        StockQuantity = p.StockQuantity,
                        CreatedDate = p.CreatedDate,
                        UpdatedDate = p.UpdatedDate
                    }).ToList()
                };

                subcategoryDTOs.Add(subcategoryDTO);
            }

            // Get the products for the main category
            var mainCategoryProducts = await _unitOfWork.Product.GetByCategoryAsync(mainCategory.Id);

            // Map the main category and its products to the DTO
            var mainCategoryDTO = new CategoryResponseDTO
            {
                Id = mainCategory.Id,
                Name = mainCategory.Name,
                Description = mainCategory.Description,
                ParentCategoryId = mainCategory.ParentCategoryId,
                Status = mainCategory.Status.ToString(),
                CreatedDate = mainCategory.CreatedDate,
                UpdatedDate = mainCategory.UpdatedDate,
                Products = mainCategoryProducts.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    status = p.Status.ToString(),
                    StockQuantity = p.StockQuantity,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate
                }).ToList(),
                Subcategories = subcategoryDTOs // Include the subcategories
            };

            return mainCategoryDTO;
        }

    }



}
