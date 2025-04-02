using ProductService.DTos;
using ProductService.Models;
using ProductService.Repositories.IRepository;
using ProductService.Services.IService;

namespace ProductService.Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => MapToCategoryDto(c));
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            return category != null ? MapToCategoryDto(category) : null;
        }

        public async Task<CategoryDto> AddCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddCategoryAsync(category);
            _logger.LogInformation($"Category {category.Id} created");

            return MapToCategoryDto(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                throw new ArgumentException($"Category with ID {id} not found");
            }

            // Update properties if provided
            if (!string.IsNullOrEmpty(categoryDto.Name))
                existingCategory.Name = categoryDto.Name;

            if (!string.IsNullOrEmpty(categoryDto.Description))
                existingCategory.Description = categoryDto.Description;

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
            _logger.LogInformation($"Category {id} updated");

            return MapToCategoryDto(existingCategory);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                throw new ArgumentException($"Category with ID {id} not found");
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            _logger.LogInformation($"Category {id} deleted");
        }

        private CategoryDto MapToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt
            };
        }
    }
}
