using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos.CategoryDtos;
using MyBudget.Application.Dtos.ExpenseDtos;
using MyBudget.Application.Interfaces;
using MyBudget.Domain.Models;
using MyBudget.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace MyBudget.Infrastructure.Services;

public class ExpensesService : IExpensesService
{
    private readonly MyBudgetDbContext _context;
    private readonly ICategoryService _categoryService;

    public ExpensesService(MyBudgetDbContext context, ICategoryService categoryService)
    {
        _context = context;
        _categoryService = categoryService;
    }

    public async Task<ExpenseDto> AddAsync(Guid userId, CreateExpenseDto dto)
    {
        if (dto.Amount <= 0)
            throw new ValidationException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.CurrencyCode))
            throw new ValidationException("Currency code is required.");

        if (string.IsNullOrWhiteSpace(dto.CategoryName))
            throw new ValidationException("Category name is required.");

        var categoryName = dto.CategoryName.Trim();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == categoryName);

        if (category == null)
        {
            var created = await _categoryService.AddAsync(userId, new CreateCategoryDto { Name = categoryName });
            category = await _context.Categories.FirstAsync(c => c.Id == created.Id);
        }

        var money = new Money(dto.Amount, dto.CurrencyCode);

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = category.Id,
            Money = money,
            Date = dto.Date,
            Note = dto.Note
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return new ExpenseDto(
            expense.Id,
            expense.CategoryId,
            expense.Money.Amount,
            expense.Money.CurrencyCode,
            expense.Date,
            expense.Note
        );
    }

    public async Task<IReadOnlyList<ExpenseDto>> GetAllAsync(Guid userId, ExpenseQueryParamsDto p)
    {
        IQueryable<Expense> q = _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId);

        if (!string.IsNullOrWhiteSpace(p.CategoryName))
        {
            var name = p.CategoryName.Trim();

            var categoryId = await _context.Categories
                .AsNoTracking()
                .Where(c => c.UserId == userId && c.Name == name)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (categoryId == Guid.Empty)
                return Array.Empty<ExpenseDto>();

            q = q.Where(e => e.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(p.CurrencyCode))
        {
            var code = p.CurrencyCode.Trim().ToUpper();
            q = q.Where(e => e.Money.CurrencyCode == code);
        }

        if (p.Amount.HasValue)
            q = q.Where(e => e.Money.Amount == p.Amount.Value);

        if (p.MinAmount.HasValue)
            q = q.Where(e => e.Money.Amount >= p.MinAmount.Value);

        if (p.MaxAmount.HasValue)
            q = q.Where(e => e.Money.Amount <= p.MaxAmount.Value);

        if (p.FromDate.HasValue)
            q = q.Where(e => e.Date >= p.FromDate.Value);

        if (p.ToDate.HasValue)
            q = q.Where(e => e.Date <= p.ToDate.Value);

        var sortBy = (p.SortBy ?? "date").Trim().ToLowerInvariant();
        q = sortBy switch
        {
            "amount" => p.SortDesc
                ? q.OrderByDescending(e => e.Money.Amount)
                : q.OrderBy(e => e.Money.Amount),

            _ => p.SortDesc
                ? q.OrderByDescending(e => e.Date)
                : q.OrderBy(e => e.Date)
        };

        var page = p.Page < 1 ? 1 : p.Page;
        var pageSize = p.SafePageSize < 1 ? 20 : p.SafePageSize;

        return await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new ExpenseDto(
                e.Id,
                e.CategoryId,
                e.Money.Amount,
                e.Money.CurrencyCode,
                e.Date,
                e.Note
            ))
            .ToListAsync();
    }

    public async Task<ExpenseDto> GetAsync(Guid userId, Guid expenseId)
    {
        var expense = await _context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);

        if (expense == null)
            throw new NotFoundException("Expense not found");

        return new ExpenseDto(
            expense.Id,
            expense.CategoryId,
            expense.Money.Amount,
            expense.Money.CurrencyCode,
            expense.Date,
            expense.Note
        );
    }

    public async Task DeleteAsync(Guid userId, Guid expenseId)
    {
        var toDelete = await _context.Expenses
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);

        if (toDelete == null)
            throw new NotFoundException("Expense not found");

        _context.Expenses.Remove(toDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<ExpenseDto> UpdateAsync(Guid userId, Guid expenseId, UpdateExpenseDto dto)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);

        if (expense == null)
            throw new NotFoundException("Expense not found");

        if (!string.IsNullOrWhiteSpace(dto.CategoryName))
        {
            var categoryName = dto.CategoryName.Trim();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == categoryName);

            if (category == null)
            {
                var created = await _categoryService.AddAsync(userId, new CreateCategoryDto { Name = categoryName });
                category = await _context.Categories.FirstAsync(c => c.Id == created.Id);
            }

            expense.CategoryId = category.Id;
        }

        if (dto.Amount.HasValue || !string.IsNullOrWhiteSpace(dto.CurrencyCode))
        {
            var newAmount = dto.Amount ?? expense.Money.Amount;
            var newCode = !string.IsNullOrWhiteSpace(dto.CurrencyCode)
                ? dto.CurrencyCode!
                : expense.Money.CurrencyCode;

            if (dto.Amount.HasValue && dto.Amount.Value <= 0)
                throw new ValidationException("Amount must be greater than zero.");

            expense.Money = new Money(newAmount, newCode);
        }

        if (dto.Date.HasValue)
            expense.Date = dto.Date.Value;

        if (dto.Note != null)
            expense.Note = dto.Note;

        await _context.SaveChangesAsync();

        return new ExpenseDto(
            expense.Id,
            expense.CategoryId,
            expense.Money.Amount,
            expense.Money.CurrencyCode,
            expense.Date,
            expense.Note
        );
    }
}
