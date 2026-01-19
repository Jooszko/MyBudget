using MyBudget.Application.Dtos.CategoryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyBudget.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(Guid userId);
        Task<CategoryDto> GetAsync(Guid userId, Guid categoryId);
        Task<CategoryDto> AddAsync(Guid userId, CreateCategoryDto categoryDto);
        Task<CategoryDto> UpdateAsync(Guid userId, Guid categoryId, UpdateCategoryDto categoryDto);
        Task DeleteAsync(Guid userId, Guid categoryId);

    }
}
