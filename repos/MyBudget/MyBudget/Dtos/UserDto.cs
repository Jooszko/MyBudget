using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Dtos
{
    public class UserDto 
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Currency { get; set; }
    }
}
