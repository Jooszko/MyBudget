using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Models
{
    public class Money
    {
        public decimal Amount { get; private set; }
        public string CurrencyCode { get; private set; }

        public Money(decimal amount, string currencyCode)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required.");

            if (currencyCode.Length != 3)
                throw new ArgumentException("Currency code must be ISO 4217 (e.g. PLN, USD).");

            Amount = amount;
            CurrencyCode = currencyCode.ToUpper();
        }
    }



}
