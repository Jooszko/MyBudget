using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Models
{
    public class Income
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Money Money { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
    }
}
