using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Dtos.IncomeDtos
{
    public class CreateIncomeDto
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
    }
}
