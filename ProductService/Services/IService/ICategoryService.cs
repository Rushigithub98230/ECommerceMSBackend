using EComMSSharedLibrary.Models;
using ProductService.DTos;

namespace ProductService.Services.IService
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync();
        Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto categoryDto);
        Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto categoryDto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
