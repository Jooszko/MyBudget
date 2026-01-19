using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos.CategoryDtos;
using MyBudget.Application.Dtos.ExpenseDtos;
using MyBudget.Application.Interfaces;
using MyBudget.Domain.Models;
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
                var created = await _categoryService.AddAsync(userId, new CreateCategoryDto { Name = createExpenseDto.CategoryName });
                category = await _context.Categories.FirstAsync(c => c.Id == created.Id);
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

        public async Task DeleteAsync(Guid userId, Guid expenseId)
        {
            var toDelete = await _context.Expenses.FirstOrDefaultAsync(e=>e.Id == expenseId && e.UserId == userId);
            if (toDelete == null)
            {
                throw new NotFoundException("Expense not found");
            }
            _context.Expenses.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ExpenseDto>> GetAllAsync(Guid userId)
        {
            var userExpanses = await _context.Expenses.AsNoTracking().Where(e => e.UserId == userId).Select(e => new ExpenseDto
            (
                e.Id,
                e.CategoryId,
                e.Amount,
                e.Date,
                e.Note
            )).ToListAsync();

            return userExpanses;
        }

        public async Task<ExpenseDto> GetAsync(Guid userId, Guid expenseId)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);
            if (expense == null)
                throw new NotFoundException("Expense not found");
            var result = new ExpenseDto(
                expense.Id,
                expense.CategoryId,
                expense.Amount,
                expense.Date,
                expense.Note
            );
            return result;
        }

        public async Task<ExpenseDto> UpdateAsync(Guid userId, Guid expenseId, UpdateExpenseDto dto)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);

            if (expense == null)
                throw new NotFoundException("Expense not found");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == dto.CategoryName);

            if (category == null)
            {
                var created = await _categoryService.AddAsync(userId, new CreateCategoryDto { Name = dto.CategoryName });
                category = await _context.Categories.FirstAsync(c => c.Id == created.Id);
            }

            expense.CategoryId = category.Id;
            expense.Amount = dto.Amount;
            expense.Date = dto.Date;
            expense.Note = dto.Note;

            await _context.SaveChangesAsync();

            return new ExpenseDto(
                expense.Id,
                expense.CategoryId,
                expense.Amount,
                expense.Date,
                expense.Note
            );
        }
    }
}
