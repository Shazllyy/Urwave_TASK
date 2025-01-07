using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Uow;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<CategoryService>> _mockLogger;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            // Setup mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<CategoryService>>();

            // Create instance of CategoryService
            _categoryService = new CategoryService(_mockUnitOfWork.Object, _mockLogger.Object, null);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                new Category { Id = Guid.NewGuid(), Name = "Category 2" }
            };
            _mockUnitOfWork.Setup(x => x.Category.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockUnitOfWork.Verify(x => x.Category.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddCategoryAsync_AddsCategory()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "New Category" };

            // Act
            await _categoryService.AddCategoryAsync(category);

            // Assert
            _mockUnitOfWork.Verify(x => x.Category.AddAsync(category), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesCategory()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Updated Category" };

            // Act
            await _categoryService.UpdateCategoryAsync(category);

            // Assert
            _mockUnitOfWork.Verify(x => x.Category.Update(category), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_CategoryDoesNotExist_ThrowsException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockUnitOfWork.Setup(x => x.Category.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _categoryService.DeleteCategoryAsync(categoryId));
            Assert.Equal("Category not found", exception.Message);
        }

        [Fact]
        public async Task DeleteCategoryAsync_CanDeleteCategory_DeletesCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Category to Delete" };
            _mockUnitOfWork.Setup(x => x.Category.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _mockUnitOfWork.Setup(x => x.Category.CanDeleteCategoryAsync(categoryId)).ReturnsAsync(true);

            // Act
            await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            _mockUnitOfWork.Verify(x => x.Category.DeleteCategoryAsync(categoryId, null), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
