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
        Task<IReadOnlyList<ExpenseDto>> GetAllAsync(Guid userId);
        Task<ExpenseDto> AddAsync(Guid userId, CreateExpenseDto createExpenseDto);
        Task<ExpenseDto> GetAsync(Guid userId, Guid expenseId);
        Task DeleteAsync(Guid userId, Guid expenseId);
        Task UpdateAsync(Guid userId, UpdateExpenseDto dto);
    }
}
