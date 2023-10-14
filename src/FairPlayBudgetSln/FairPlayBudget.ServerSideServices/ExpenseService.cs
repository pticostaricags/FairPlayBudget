using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.Expense;
using Microsoft.EntityFrameworkCore;

namespace FairPlayBudget.ServerSideServices
{
    public class ExpenseService : IExpenseService
    {
        private readonly FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext;
        private readonly IUserProviderService userProvider;

        public ExpenseService(FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext,
            IUserProviderService userProvider)
        {
            this.fairPlayBudgetDatabaseContext = fairPlayBudgetDatabaseContext;
            this.userProvider = userProvider;
        }
        public async Task CreateExpenseAsync(CreateExpenseModel createExpenseModel)
        {
            Expense entity = new Expense()
            {
                AmountInUsd=createExpenseModel.AmountInUsd!.Value,
                Description=createExpenseModel.Description,
                ExpenseDateTime=createExpenseModel.ExpenseDateTime!.Value,
                OwnerId = this.userProvider.GetCurrentUserId(),
            };
            await this.fairPlayBudgetDatabaseContext.Expense.AddAsync(entity);
            await this.fairPlayBudgetDatabaseContext.SaveChangesAsync();
        }

        public async Task<MyExpenseModel[]> GetMyExpensesAsync()
        {
            var userId = this.userProvider.GetCurrentUserId();
            var result = await this.fairPlayBudgetDatabaseContext.Expense!
                .Where(p => p.OwnerId == userId)
                .Select(p=> new MyExpenseModel()
                {
                    AmountInUsd = p.AmountInUsd,
                    Description=p.Description,
                    ExpenseDateTime=p.ExpenseDateTime,
                }).ToArrayAsync();
            return result;
        }
    }
}
