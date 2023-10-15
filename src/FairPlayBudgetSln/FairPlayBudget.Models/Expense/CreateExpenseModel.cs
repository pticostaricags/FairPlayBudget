using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayBudget.Common.Enums;

namespace FairPlayBudget.Models.Expense
{
    public class CreateExpenseModel
    {
        [Required]
        public DateTimeOffset? ExpenseDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string? Description { get; set; }

        [Required]
        public decimal? Amount { get; set; }
        [Required]
        public Currency? Currency { get; set; }
    }
}
