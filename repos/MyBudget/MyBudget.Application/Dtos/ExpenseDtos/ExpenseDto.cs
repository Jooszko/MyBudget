using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Dtos.ExpenseDtos
{
    public record ExpenseDto(
        Guid Id,
        Guid CategoryId,
        decimal Amount,
        DateTime Date,
        string? Note
    );
}
