using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Dtos.IncomeDtos
{
    public class IncomeQueryParamsDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Source { get; set; }
        public decimal? Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortBy { get; set; } = "date";
        public bool SortDesc { get; set; } = true;

        public int SafePageSize => PageSize > 50 ? 50 : PageSize;
    }
}
