using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos.IncomeDtos;
using MyBudget.Application.Interfaces;
using MyBudget.Domain.Models;
using MyBudget.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Infrastructure.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly MyBudgetDbContext _context;
        public IncomeService(MyBudgetDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<IncomeDto>> GetAllAsync(Guid userId)
        {
            var incomes = await _context.Incomes.AsNoTracking().Where(i=>i.UserId==userId)
                .Select(i=> new IncomeDto(
                    i.Id,
                    i.UserId,
                    i.Amount,
                    i.Date,
                    i.Source
                    )).ToListAsync();
            return incomes;
        }

        public async Task<IncomeDto> GetAsync(Guid userId, Guid incomeId)
        {
            var income = await _context.Incomes.AsNoTracking().FirstOrDefaultAsync(i=>i.UserId == userId && i.Id == incomeId);
            if (income == null)
            {
                throw new NotFoundException("Income not found");
            }
            return new IncomeDto(
                income.Id,
                income.UserId,
                income.Amount,
                income.Date,
                income.Source
                );
        }

        public async Task<IncomeDto> AddAsync(Guid userId, CreateIncomeDto dto)
        {
            if (dto.Amount <= 0)
                throw new ValidationException("Amount must be greater than zero.");

            var income = new Income
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = dto.Amount,
                Date = dto.Date,
                Source = dto.Source
            };

            await _context.Incomes.AddAsync(income);
            await _context.SaveChangesAsync();

            return new IncomeDto(
                income.Id,
                income.UserId,
                income.Amount,
                income.Date,
                income.Source
                );
        }

        public async Task DeleteAsync(Guid userId, Guid incomeId)
        {
            var toDelete = await _context.Incomes.FirstOrDefaultAsync(i=>i.UserId == userId && i.Id == incomeId);
            if (toDelete == null)
            {
                throw new NotFoundException("Income not found");
            }

            _context.Incomes.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IncomeDto> UpdateAsync(Guid userId, Guid incomeId, UpdateIncomeDto dto)
        {
            if (dto.Amount <= 0)
                throw new ValidationException("Amount must be greater than zero.");

            var toUpdate = await _context.Incomes.FirstOrDefaultAsync(i => i.UserId == userId && i.Id == incomeId);
            if (toUpdate == null)
            {
                throw new NotFoundException("Income not found");
            }


            toUpdate.Amount = dto.Amount;
            toUpdate.Date = dto.Date;
            toUpdate.Source = dto.Source;

            await _context.SaveChangesAsync();

            return new IncomeDto(
                toUpdate.Id,
                toUpdate.UserId,
                toUpdate.Amount,
                toUpdate.Date,
                toUpdate.Source
                );
        }
    }
}
