using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayBudget.Common.Enums;

namespace FairPlayBudget.Models.Balance
{
    public class MyBalanceModel
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        [StringLength(10)]
        public string? TransactionType { get; set; }

        public DateTimeOffset DateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string? Description { get; set; }
        [Required]
        public string? MonthlyBudgetDescription { get; set; }
    }
}
