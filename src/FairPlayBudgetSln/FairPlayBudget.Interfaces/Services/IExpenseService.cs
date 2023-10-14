using FairPlayBudget.Models.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Interfaces.Services
{
    public interface IExpenseService
    {
        Task CreateExpenseAsync(CreateExpenseModel createExpenseModel, CancellationToken cancellationToken);
        Task<MyExpenseModel[]> GetMyExpensesAsync(CancellationToken cancellationToken);
    }
}
