using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Common
{
    public class ConflictException : Exception
    {
        public ConflictException(string message):base(message) { }
    }
}
