using FairPlayBudget.Common.Enums;
using FairPlayBudget.Models.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Interfaces.Services
{
    public interface IBalanceService
    {
        Task<MyBalanceModel[]> GetMyBalanceAsync(
            string budgetName, Currency currency,CancellationToken cancellationToken);
        Task<string[]> GetBudgetNamesAsync(CancellationToken cancellationToken);
    }
}
