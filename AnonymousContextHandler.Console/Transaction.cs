using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnonymousContextHandler.Console
{
    public class Transaction :TableEntity
    {
        public Transaction()
        {
            
        }

        public Transaction(string description, decimal amount, int installment,DateTime date)
        {
            Description = description;
            Amount = amount;
            Installment = installment;
            Date = date;
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Installment { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount => Amount * Installment;
    }
}
