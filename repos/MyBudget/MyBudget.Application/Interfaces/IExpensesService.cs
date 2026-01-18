using MyBudget.Application.Dtos;
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
    }
}
