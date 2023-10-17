using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.ServerSideServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.AutomatedTests.ServerSideServices
{
    public abstract class ServerSideServicesTestBase
    {
        protected const string DefaultConnectionString = "Data Source=(local);Initial Catalog=FairPlayBudget;Integrated Security=True;Trust Server Certificate=True";
        public static string? CurrentUserId { get; protected set; }

        protected FairPlayBudgetDatabaseContext GetFairPlayBudgetDatabaseContext()
        {
            DbContextOptionsBuilder<FairPlayBudgetDatabaseContext> dbContextOptionsBuilder =
                new DbContextOptionsBuilder<FairPlayBudgetDatabaseContext>();
            dbContextOptionsBuilder.UseSqlServer(DefaultConnectionString,
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(3),
                            errorNumbersToAdd: null);
                });
            FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext =
                new FairPlayBudgetDatabaseContext(dbContextOptionsBuilder.Options);
            return fairPlayBudgetDatabaseContext;
        }

        private IUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }

        protected IBalanceService GetBalanceServiceInstance()
        {
            FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext = this.GetFairPlayBudgetDatabaseContext();
            IUserProviderService userProviderService = this.GetUserProviderService();
            return new BalanceService(fairPlayBudgetDatabaseContext, userProviderService);
        }
    }
}
