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
        public async Task CreateExpenseAsync(CreateExpenseModel createExpenseModel,
            CancellationToken cancellationToken)
        {
            Expense entity = new Expense()
            {
                AmountInUsd=createExpenseModel.AmountInUsd!.Value,
                Description=createExpenseModel.Description,
                ExpenseDateTime=createExpenseModel.ExpenseDateTime!.Value,
                OwnerId = this.userProvider.GetCurrentUserId(),
            };
            await this.fairPlayBudgetDatabaseContext.Expense.AddAsync(entity,
                cancellationToken:cancellationToken);
            await this.fairPlayBudgetDatabaseContext.SaveChangesAsync(
                cancellationToken:cancellationToken);
        }

        public async Task<MyExpenseModel[]> GetMyExpensesAsync(CancellationToken cancellationToken)
        {
            var userId = this.userProvider.GetCurrentUserId();
            var result = await this.fairPlayBudgetDatabaseContext.Expense!
                .Where(p => p.OwnerId == userId)
                .Select(p=> new MyExpenseModel()
                {
                    AmountInUsd = p.AmountInUsd,
                    Description=p.Description,
                    ExpenseDateTime=p.ExpenseDateTime,
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<MyExpenseModel[]> GetMyExpensesForMonthAsync(int year, int month, CancellationToken cancellationToken)
        {
            if (month <= 0 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), month,
                    $"Month value must be between 1 and 12");
            var userId = this.userProvider.GetCurrentUserId();
            var result = await this.fairPlayBudgetDatabaseContext.Expense!
                .Where(p => p.OwnerId == userId &&
                p.ExpenseDateTime.Month == month && 
                p.ExpenseDateTime.Year == year)
                .Select(p => new MyExpenseModel()
                {
                    AmountInUsd = p.AmountInUsd,
                    Description = p.Description,
                    ExpenseDateTime = p.ExpenseDateTime,
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
