using FairPlayBudget.Models.Expense;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Interfaces.Services
{
    public interface IExpenseService
    {
        Task CreateExpenseAsync(CreateExpenseModel createExpenseModel, CancellationToken cancellationToken);
        Task<MyExpenseModel[]> GetMyExpensesAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month">1 for January, 12 for December</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MyExpenseModel[]> GetMyExpensesForMonthAsync(int year, int month,CancellationToken cancellationToken);
    }
}
