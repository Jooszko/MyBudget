using MyBudget.Application.Dtos.CategoryDtos;
using MyBudget.Application.Dtos.IncomeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Interfaces
{
    public interface IIncomeService
    {
        Task<IReadOnlyList<IncomeDto>> GetAllAsync(Guid userId, IncomeQueryParamsDto q);
        Task<IncomeDto> GetAsync(Guid userId, Guid IncomeId);
        Task<IncomeDto> AddAsync(Guid userId, CreateIncomeDto dto);
        Task<IncomeDto> UpdateAsync(Guid userId, Guid incomeId, UpdateIncomeDto dto);
        Task DeleteAsync(Guid userId, Guid incomeId);
    }
}
