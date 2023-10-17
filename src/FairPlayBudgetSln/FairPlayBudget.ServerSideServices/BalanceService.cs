using FairPlayBudget.Common.Enums;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.Balance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.ServerSideServices
{
    public class BalanceService(FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext,
        IUserProviderService userProvider) : IBalanceService
    {
        public async Task<string[]> GetBudgetNamesAsync(CancellationToken cancellationToken)
        {
            var result = await fairPlayBudgetDatabaseContext!
                .MonthlyBudgetInfo.Select(p => p.Description).Distinct().ToArrayAsync(
                cancellationToken: cancellationToken);
            return result;
        }

        public async Task<MyBalanceModel[]> GetMyBalanceAsync(
            string budgetName,
            Currency currency,
            CancellationToken cancellationToken)
        {
            var userId = userProvider.GetCurrentUserId();
            var result = await fairPlayBudgetDatabaseContext.VwBalance
                .Where(p => p.OwnerId == userId && p.MonthlyBudgetDescription == budgetName
                && p.CurrencyId == (int)currency
                )
                .Select(p => new MyBalanceModel()
                {
                    Amount = p.Amount,
                    Currency = (Currency)p.CurrencyId,
                    DateTime = p.DateTime,
                    Description = p.Description,
                    TransactionType = p.TransactionType,
                    MonthlyBudgetDescription = p.MonthlyBudgetDescription
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
