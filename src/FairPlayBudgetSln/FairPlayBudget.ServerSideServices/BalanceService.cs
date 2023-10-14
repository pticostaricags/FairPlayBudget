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
    public class BalanceService : IBalanceService
    {
        private readonly FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext;
        private readonly IUserProviderService userProvider;

        public BalanceService(FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext,
            IUserProviderService userProvider)
        {
            this.fairPlayBudgetDatabaseContext = fairPlayBudgetDatabaseContext;
            this.userProvider = userProvider;
        }
        public async Task<MyBalanceModel[]> GetMyBalanceAsync(CancellationToken cancellationToken)
        {
            var userId = this.userProvider.GetCurrentUserId();
            var result = await this.fairPlayBudgetDatabaseContext.VwBalance
                .Where(p => p.OwnerId == userId)
                .Select(p => new MyBalanceModel() 
                {
                    AmountInUsd=p.AmountInUsd,
                    DateTime=p.DateTime,
                    Description=p.Description,
                    TransactionType=p.TransactionType
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
