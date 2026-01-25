using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Dtos.IncomeDtos
{
    public record IncomeDto
    (
        Guid Id,
        Guid UserId,
        decimal Amount,
        string CurrencyCode,
        DateTime Date,
        string Source
    );
}
