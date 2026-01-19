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
            var name = categoryDto.Name.Trim();

            var exist = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == name);

            if (exist != null)
                throw new ConflictException("Category already exists.");

            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return new CategoryDto(newCategory.Id, userId, name);
        }

        public async Task DeleteAsync(Guid userId, Guid categoryId)
        {
            var toDelete = await _context.Categories.FirstOrDefaultAsync(c=>c.UserId == userId && c.Id == categoryId);
            if (toDelete == null)
            {
                throw new NotFoundException("Category not found");
            }
            _context.Categories.Remove(toDelete);
            await _context.SaveChangesAsync();
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

        public async Task<CategoryDto> GetAsync(Guid userId, Guid categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId);
            if(category == null)
                throw new NotFoundException("Category not found");

            return new CategoryDto(
                    category.Id,
                    category.UserId,
                    category.Name
                );
        }

        public async Task<CategoryDto> UpdateAsync(Guid userId, Guid categoryId, UpdateCategoryDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId);

            if (category == null)
                throw new NotFoundException("Category not found");

            var newName = dto.CategoryName.Trim();

            var exists = await _context.Categories
                .AnyAsync(c => c.UserId == userId && c.Name == newName && c.Id != categoryId);

            if (exists)
                throw new ConflictException("Category already exists.");

            category.Name = newName;
            await _context.SaveChangesAsync();

            return new CategoryDto(category.Id, category.UserId, category.Name);
        }
    }
}
