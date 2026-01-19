using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos.CategoryDtos;
using MyBudget.Application.Dtos.ExpenseDtos;
using MyBudget.Application.Interfaces;
using MyBudget.Domain.Models;
using MyBudget.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly MyBudgetDbContext _context;
        public CategoryService(MyBudgetDbContext context)
        {
            _context = context;
        }
        public async Task<CategoryDto> AddAsync(Guid userId, CreateCategoryDto categoryDto)
        {
            var exist = await _context.Categories.FirstOrDefaultAsync(c=>c.Name == categoryDto.Name && c.UserId == userId);
            if (exist != null)
            {
                throw new ConflictException("Category already exists.");
            }
            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = categoryDto.Name,
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return new CategoryDto
            (
                newCategory.Id,
                userId,
                categoryDto.Name
            );
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(Guid userId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryDto(
                    c.Id,
                    c.UserId,
                    c.Name
                ))
                .ToListAsync();
        }
    }
}
