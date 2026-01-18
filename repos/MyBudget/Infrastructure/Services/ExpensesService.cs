using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos;
using MyBudget.Application.Interfaces;
using MyBudget.Domain.Models;
using MyBudget.Dtos.CategoryDtos;
using MyBudget.Infrastructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Infrastructure.Services
{
    public class ExpensesService : IExpensesService
    {
        private readonly MyBudgetDbContext _context;
        private readonly ICategoryService _categoryService;
        public ExpensesService(MyBudgetDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task<ExpenseDto> AddAsync(Guid userId,CreateExpenseDto createExpenseDto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == createExpenseDto.CategoryName);
            if (category == null)
            {
                var createdCategory = await _categoryService.AddAsync(userId, new CreateCategoryDto
                {
                    Name = createExpenseDto.CategoryName,
                });
                category = new Category
                {
                    Id = createdCategory.Id,
                    UserId = userId,
                    Name = createExpenseDto.CategoryName
                };
            }
            
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = category.Id,
                Amount = createExpenseDto.Amount,
                Date = createExpenseDto.Date,
                Note = createExpenseDto.Note,
            };
            await _context.Expenses.AddAsync(expense);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Failed to create expense.", ex);
            }

            return new ExpenseDto
            (
                expense.Id,
                expense.CategoryId,
                expense.Amount,
                expense.Date,
                expense.Note
            );
        }

        public async Task<IReadOnlyList<ExpenseDto>> GetAllAsync(Guid userId)
        {
            var userExpanses = await _context.Expenses.Where(e => e.UserId == userId).Select(e => new ExpenseDto
            (
                e.Id,
                e.CategoryId,
                e.Amount,
                e.Date,
                e.Note
            )).ToListAsync();

            return userExpanses;
        }
    }
}
