using FairPlayBudget.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Models.MonthlyBudgetInfo
{
    public class CreateMonthlyBudgetInfoModel
    {
        [Required]
        [StringLength(150)]
        public string? Description { get; set; }
        [Required]
        [ValidateComplexType]
        public List<CreateTransactionModel>? Transactions { get; set; }
    }

    public class CreateTransactionModel
    {
        [Required]
        public DateTimeOffset? TransactionDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string? Description { get; set; }

        [Required]
        public decimal? AmountInUsd { get; set; }
        [Required]
        public TransactionType? TransactionType { get; set; }
    }
}
