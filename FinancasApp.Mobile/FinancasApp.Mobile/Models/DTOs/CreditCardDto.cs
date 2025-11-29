using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancasApp.Mobile.Models.DTOs;
    public class CreditCardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Last4Digits { get; set; } = "";
        public decimal CreditLimit { get; set; }

        public Guid? CurrentInvoiceId { get; set; }

        public int DueDay { get; set; }
        public int ClosingDay { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

