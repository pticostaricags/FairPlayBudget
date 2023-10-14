using FairPlayBudget.Models.Expense;
using FairPlayBudget.Models.Income;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Interfaces.Services
{
    public interface IIncomeService
    {
        Task CreateIncomeAsync(CreateIncomeModel createExpenseModel, CancellationToken cancellationToken);
    }
}
