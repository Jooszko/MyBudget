using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos.IncomeDtos;
using MyBudget.Application.Interfaces;
using MyBudget.Domain.Models;
using MyBudget.Infrastructure.Data;

namespace MyBudget.Infrastructure.Services;

public class IncomeService : IIncomeService
{
    private readonly MyBudgetDbContext _context;

    public IncomeService(MyBudgetDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<IncomeDto>> GetAllAsync(Guid userId, IncomeQueryParamsDto p)
    {
        IQueryable<Income> q = _context.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId);

        if (!string.IsNullOrWhiteSpace(p.Source))
        {
            var source = p.Source.Trim();
            q = q.Where(i => i.Source == source);
        }

        if (!string.IsNullOrWhiteSpace(p.CurrencyCode))
        {
            var code = p.CurrencyCode.Trim().ToUpper();
            q = q.Where(i => i.Money.CurrencyCode == code);
        }

        if (p.Amount.HasValue)
            q = q.Where(i => i.Money.Amount == p.Amount.Value);

        if (p.FromDate.HasValue)
            q = q.Where(i => i.Date >= p.FromDate.Value);

        if (p.ToDate.HasValue)
            q = q.Where(i => i.Date <= p.ToDate.Value);

        var sortBy = (p.SortBy ?? "date").Trim().ToLowerInvariant();
        q = sortBy switch
        {
            "amount" => p.SortDesc
                ? q.OrderByDescending(i => i.Money.Amount)
                : q.OrderBy(i => i.Money.Amount),

            "source" => p.SortDesc
                ? q.OrderByDescending(i => i.Source)
                : q.OrderBy(i => i.Source),

            _ => p.SortDesc
                ? q.OrderByDescending(i => i.Date)
                : q.OrderBy(i => i.Date)
        };

        var page = p.Page < 1 ? 1 : p.Page;
        var pageSize = p.SafePageSize < 1 ? 20 : p.SafePageSize;

        return await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new IncomeDto(
                i.Id,
                i.UserId,
                i.Money.Amount,
                i.Money.CurrencyCode,
                i.Date,
                i.Source
            ))
            .ToListAsync();
    }

    public async Task<IncomeDto> GetAsync(Guid userId, Guid incomeId)
    {
        var income = await _context.Incomes
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Id == incomeId);

        if (income == null)
            throw new NotFoundException("Income not found");

        return new IncomeDto(
            income.Id,
            income.UserId,
            income.Money.Amount,
            income.Money.CurrencyCode,
            income.Date,
            income.Source
        );
    }

    public async Task<IncomeDto> AddAsync(Guid userId, CreateIncomeDto dto)
    {
        var money = new Money(dto.Amount, dto.CurrencyCode);

        var income = new Income
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Money = money,          
            Date = dto.Date,
            Source = dto.Source
        };

        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        return new IncomeDto(
            income.Id,
            income.UserId,
            income.Money.Amount,
            income.Money.CurrencyCode,
            income.Date,
            income.Source
        );
    }

    public async Task DeleteAsync(Guid userId, Guid incomeId)
    {
        var toDelete = await _context.Incomes
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Id == incomeId);

        if (toDelete == null)
            throw new NotFoundException("Income not found");

        _context.Incomes.Remove(toDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<IncomeDto> UpdateAsync(Guid userId, Guid incomeId, UpdateIncomeDto dto)
    {
        var income = await _context.Incomes
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Id == incomeId);

        if (income == null)
            throw new NotFoundException("Income not found");

        if (dto.Amount.HasValue || !string.IsNullOrWhiteSpace(dto.CurrencyCode))
        {
            var newAmount = dto.Amount ?? income.Money.Amount;
            var newCode = !string.IsNullOrWhiteSpace(dto.CurrencyCode)
                ? dto.CurrencyCode!
                : income.Money.CurrencyCode;

            income.Money = new Money(newAmount, newCode);
        }

        if (dto.Date.HasValue)
            income.Date = dto.Date.Value;

        if (dto.Source != null)
            income.Source = dto.Source;

        await _context.SaveChangesAsync();

        return new IncomeDto(
            income.Id,
            income.UserId,
            income.Money.Amount,
            income.Money.CurrencyCode,
            income.Date,
            income.Source
        );
    }
}
