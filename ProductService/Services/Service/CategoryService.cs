using EComMSSharedLibrary.Models;
using ProductService.Data.DataAccessRepositories.categoryRepositories;
using ProductService.DTos;
using ProductService.Models;
using ProductService.Services.IService;

namespace ProductService.Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || !category.IsActive)
                return ApiResponse<CategoryDto>.ErrorResponse("Category not found", 404);

            return ApiResponse<CategoryDto>.SuccessResponse(MapToDto(category));
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = categories.Select(MapToDto);
            return ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos);
        }

        public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);
            return ApiResponse<CategoryDto>.SuccessResponse(MapToDto(createdCategory), "Category created successfully", 201);
        }

        public async Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || !category.IsActive)
                return ApiResponse<CategoryDto>.ErrorResponse("Category not found", 404);

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return ApiResponse<CategoryDto>.SuccessResponse(MapToDto(updatedCategory));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var result = await _categoryRepository.DeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("Category not found", 404);

            return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully");
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
