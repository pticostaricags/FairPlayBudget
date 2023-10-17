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
using Testcontainers.MsSql;

namespace FairPlayBudget.AutomatedTests.ServerSideServices
{
    public abstract class ServerSideServicesTestBase
    {
        public static string? CurrentUserId { get; protected set; }
        public static readonly MsSqlContainer _msSqlContainer
        = new MsSqlBuilder().Build();
        protected async Task<FairPlayBudgetDatabaseContext> GetFairPlayBudgetDatabaseContextAsync()
        {
            DbContextOptionsBuilder<FairPlayBudgetDatabaseContext> dbContextOptionsBuilder =
                new DbContextOptionsBuilder<FairPlayBudgetDatabaseContext>();
            dbContextOptionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString(),
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(3),
                            errorNumbersToAdd: null);
                });
            FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext =
                new FairPlayBudgetDatabaseContext(dbContextOptionsBuilder.Options);
            await fairPlayBudgetDatabaseContext.Database.EnsureCreatedAsync();
            await fairPlayBudgetDatabaseContext.Database.ExecuteSqlRawAsync(Properties.Resources.SeedData);
            return fairPlayBudgetDatabaseContext;
        }

        private IUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }

        internal async Task<IBalanceService> GetBalanceServiceInstanceAsync()
        {
            FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext = 
                await this.GetFairPlayBudgetDatabaseContextAsync();
            IUserProviderService userProviderService = this.GetUserProviderService();
            return new BalanceService(fairPlayBudgetDatabaseContext, userProviderService);
        }

        internal async Task<IMonthlyBudgetInfoService> GetMonthlyBudgetInfoServiceAsync()
        {
            FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext =
                await this.GetFairPlayBudgetDatabaseContextAsync();
            IUserProviderService userProviderService = this.GetUserProviderService();
            return new MonthlyBudgetInfoService(fairPlayBudgetDatabaseContext, userProviderService);
        }
    }
}
