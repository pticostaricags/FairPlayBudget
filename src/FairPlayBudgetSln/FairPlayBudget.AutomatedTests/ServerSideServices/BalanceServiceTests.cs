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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
        }

        [ClassCleanup()]
        public static async Task ClassCleanupAsync()
        {
            if (ServerSideServicesTestBase._msSqlContainer.State == DotNet.Testcontainers.Containers.TestcontainersStates.Running)
            {
                await ServerSideServicesTestBase._msSqlContainer.StopAsync();
            }
        }

        [TestCleanup]
        public async Task TestCleanupAsync()
        {
            var ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            foreach (var expense in ctx.Expense)
            {
                ctx.Expense.Remove(expense);
            }
            foreach (var income in ctx.Income)
            {
                ctx.Income.Remove(income);
            }
            foreach (var monthlyBudgetInfo in ctx.MonthlyBudgetInfo)
            {
                ctx.MonthlyBudgetInfo.Remove(monthlyBudgetInfo);
            }
            foreach (var aspNetUser in ctx.AspNetUsers)
            {
                ctx.AspNetUsers.Remove(aspNetUser);
            }
            await ctx.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Test_GetBudgetNamesAsync()
        {
            var ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            AspNetUsers userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            string budgetName = "Automated Test #1";
            MonthlyBudgetInfo monthlyBudgetInfo = new()
            {
                Description = budgetName,
                OwnerId = ServerSideServicesTestBase.CurrentUserId
            };
            await ctx.MonthlyBudgetInfo.AddAsync(monthlyBudgetInfo);
            await ctx.SaveChangesAsync();
            IBalanceService balanceService = await ServerSideServicesTestBase.GetBalanceServiceInstanceAsync();
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
            AspNetUsers userEntity = new()
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
            var ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            AspNetUsers userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            MonthlyBudgetInfo monthlyBudgetInfo = new()
            {
                Description = $"Automated Test: {nameof(Test_GetMyBalanceAsync)}",
                OwnerId = userEntity.Id,
                Expense = new Expense[]
                {
                    new()
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
                    new()
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