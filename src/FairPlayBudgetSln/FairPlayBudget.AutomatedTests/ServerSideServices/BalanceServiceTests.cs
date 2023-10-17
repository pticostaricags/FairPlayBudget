using FairPlayBudget.Common.Enums;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using System.Diagnostics;

namespace FairPlayBudget.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class BalanceServiceTests : ServerSideServicesTestBase
    {
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
        }

        [ClassCleanup()]
        public static async Task ClassCleanup()
        {
            if (ServerSideServicesTestBase._msSqlContainer.State == DotNet.Testcontainers.Containers.TestcontainersStates.Running)
            {
                await ServerSideServicesTestBase._msSqlContainer.StopAsync();
            }
        }

        [TestMethod]
        public async Task Test_GetBudgetNamesAsync()
        {
            var ctx = await base.GetFairPlayBudgetDatabaseContextAsync();
            AspNetUsers userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            string budgetName = "Automated Test #1";
            MonthlyBudgetInfo monthlyBudgetInfo = new MonthlyBudgetInfo()
            {
                Description = budgetName,
                OwnerId = ServerSideServicesTestBase.CurrentUserId
            };
            await ctx.MonthlyBudgetInfo.AddAsync(monthlyBudgetInfo);
            await ctx.SaveChangesAsync();
            IBalanceService balanceService = await base.GetBalanceServiceInstanceAsync();
            var budgetsInfo = await balanceService.GetBudgetNamesAsync(CancellationToken.None);
            Assert.IsNotNull(budgetsInfo);
            Assert.AreEqual(1, budgetsInfo.Length);
            Assert.AreEqual(budgetName, budgetsInfo.Single());
            ctx.MonthlyBudgetInfo.Remove(monthlyBudgetInfo);
            ctx.AspNetUsers.Remove(userEntity);
            ctx.SaveChanges();
        }

        private static async Task<AspNetUsers> CreateTestUserAsync(FairPlayBudgetDatabaseContext ctx)
        {
            AspNetUsers userEntity = new AspNetUsers()
            {
                Id = Guid.NewGuid().ToString(),
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "test@test.test",
                EmailConfirmed = false,
                LockoutEnabled = false,
                NormalizedEmail = "test@test.test",
                NormalizedUserName = "test@test.test",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumber = "111-1111-1111",
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
                UserName = "test@test.test"
            };
            await ctx.AspNetUsers.AddAsync(userEntity);
            await ctx.SaveChangesAsync();
            return userEntity;
        }

        [TestMethod]
        public async Task Test_GetMyBalanceAsync()
        {
            var ctx = await base.GetFairPlayBudgetDatabaseContextAsync();
            AspNetUsers userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            MonthlyBudgetInfo monthlyBudgetInfo = new MonthlyBudgetInfo()
            {
                Description = $"Automated Test: {nameof(Test_GetMyBalanceAsync)}",
                OwnerId = userEntity.Id,
                Expense = new Expense[]
                {
                    new Expense()
                    {
                        Amount = 100,
                        CurrencyId = (int)Common.Enums.Currency.USD,
                        ExpenseDateTime = DateTimeOffset.UtcNow,
                        Description = "Test Expense",
                        OwnerId = userEntity.Id,
                    }
                },
                Income = new Income[] 
                {
                    new Income()
                    {
                        Amount = 100,
                        CurrencyId = (int)Common.Enums.Currency.USD,
                        IncomeDateTime = DateTimeOffset.UtcNow,
                        Description = "Test Income",
                        OwnerId= userEntity.Id,
                    }
                }
            };
            await ctx.MonthlyBudgetInfo.AddAsync(monthlyBudgetInfo);
            await ctx.SaveChangesAsync();
            var budgetEntity = ctx.MonthlyBudgetInfo.Single();
            Assert.AreEqual(monthlyBudgetInfo.Description, budgetEntity.Description);
        }
    }
}