using ProductService.DTos;

namespace ProductService.Services.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> AddCategoryAsync(CreateCategoryDto categoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
    }
}
