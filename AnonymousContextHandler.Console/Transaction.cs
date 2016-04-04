using System;

namespace AnonymousContextHandler.Console
{
    public class Transaction 
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
