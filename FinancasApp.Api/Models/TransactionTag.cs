using System;
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models
{
    // TransactionTag Model
    public class TransactionTag
    {
        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; }


        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
