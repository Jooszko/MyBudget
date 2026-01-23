using MyBudget.Application.Dtos.ExpenseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Interfaces
{
    public interface IExpensesService
    {
        Task<IReadOnlyList<ExpenseDto>> GetAllAsync(Guid userId, ExpenseQueryParamsDto query);
        Task<ExpenseDto> AddAsync(Guid userId, CreateExpenseDto createExpenseDto);
        Task<ExpenseDto> GetAsync(Guid userId, Guid expenseId);
        Task DeleteAsync(Guid userId, Guid expenseId);
        Task<ExpenseDto> UpdateAsync(Guid userId, Guid expenseId, UpdateExpenseDto dto);
    }
}
