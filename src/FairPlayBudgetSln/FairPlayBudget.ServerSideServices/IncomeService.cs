using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.Income;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.ServerSideServices
{
    public class IncomeService : IIncomeService
    {
        private readonly FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext;
        private readonly IUserProviderService userProvider;

        public IncomeService(FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext,
            IUserProviderService userProvider)
        {
            this.fairPlayBudgetDatabaseContext = fairPlayBudgetDatabaseContext;
            this.userProvider = userProvider;
        }
        public async Task CreateIncomeAsync(CreateIncomeModel createExpenseModel, CancellationToken cancellationToken)
        {
            Income entity = new Income()
            {
                AmountInUsd = createExpenseModel.AmountInUsd!.Value,
                Description = createExpenseModel.Description,
                IncomeDateTime = createExpenseModel.IncomeDateTime!.Value,
                OwnerId = this.userProvider.GetCurrentUserId(),
            };
            await this.fairPlayBudgetDatabaseContext.Income.AddAsync(entity,
                cancellationToken: cancellationToken);
            await this.fairPlayBudgetDatabaseContext.SaveChangesAsync(
                cancellationToken: cancellationToken);
        }
    }
}
