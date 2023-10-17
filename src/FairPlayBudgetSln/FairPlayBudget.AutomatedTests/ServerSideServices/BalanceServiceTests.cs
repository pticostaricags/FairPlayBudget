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
            var ctx = base.GetFairPlayBudgetDatabaseContext();
            await ctx.Database.EnsureCreatedAsync();
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
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            string budgetName = "Automated Test #1";
            MonthlyBudgetInfo monthlyBudgetInfo = new MonthlyBudgetInfo()
            {
                Description = budgetName,
                OwnerId = ServerSideServicesTestBase.CurrentUserId
            };
            await ctx.MonthlyBudgetInfo.AddAsync(monthlyBudgetInfo);
            await ctx.SaveChangesAsync();
            IBalanceService balanceService = base.GetBalanceServiceInstance();
            var budgetsInfo = await balanceService.GetBudgetNamesAsync(CancellationToken.None);
            Assert.IsNotNull(budgetsInfo);
            Assert.AreEqual(1, budgetsInfo.Length);
            Assert.AreEqual(budgetName, budgetsInfo.Single());
            ctx.MonthlyBudgetInfo.Remove(monthlyBudgetInfo);
            ctx.AspNetUsers.Remove(userEntity);
            ctx.SaveChanges();
        }
    }
}