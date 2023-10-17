using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.MonthlyBudgetInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class MonthlyBudgetInfoServiceTests : ServerSideServicesTestBase
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
        public async Task Test_CreateMonthlyBudgetInfoAsync()
        {
            FairPlayBudgetDatabaseContext ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IMonthlyBudgetInfoService monthlyBudgetInfoService = await ServerSideServicesTestBase.GetMonthlyBudgetInfoServiceAsync();
            CreateMonthlyBudgetInfoModel createMonthlyBudgetInfoModel = new()
            {
                Description = "Description",
                Transactions =
                [
                    new CreateTransactionModel()
                    {
                        Amount=100,
                        Currency = Common.Enums.Currency.USD,
                        Description= "Description",
                        TransactionDateTime = DateTimeOffset.UtcNow,
                        TransactionType = Common.Enums.TransactionType.Credit
                    },
                    new CreateTransactionModel()
                    {
                        Amount=100,
                        Currency = Common.Enums.Currency.USD,
                        Description= "Description 2",
                        TransactionDateTime = DateTimeOffset.UtcNow,
                        TransactionType = Common.Enums.TransactionType.Debit
                    }
                ]
            };
            await monthlyBudgetInfoService.CreateMonthlyBudgetInfoAsync(createMonthlyBudgetInfoModel, CancellationToken.None);
            MonthlyBudgetInfo entity = await ctx.MonthlyBudgetInfo.SingleAsync();
            Assert.AreEqual(createMonthlyBudgetInfoModel.Description, entity.Description);
        }

        [TestMethod]
        public async Task Test_UpdateMonthlyBudgetInfoAsync()
        {
            FairPlayBudgetDatabaseContext ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IMonthlyBudgetInfoService monthlyBudgetInfoService = await ServerSideServicesTestBase.GetMonthlyBudgetInfoServiceAsync();
            CreateMonthlyBudgetInfoModel createMonthlyBudgetInfoModel = new()
            {
                Description = "Description",
                Transactions =
                [
                    new()
                    {
                        Amount=100,
                        Currency = Common.Enums.Currency.USD,
                        Description= "Description",
                        TransactionDateTime = DateTimeOffset.UtcNow,
                        TransactionType = Common.Enums.TransactionType.Credit
                    },
                    new()
                    {
                        Amount=100,
                        Currency = Common.Enums.Currency.USD,
                        Description= "Description 2",
                        TransactionDateTime = DateTimeOffset.UtcNow,
                        TransactionType = Common.Enums.TransactionType.Debit
                    }
                ]
            };
            await monthlyBudgetInfoService.CreateMonthlyBudgetInfoAsync(createMonthlyBudgetInfoModel, CancellationToken.None);
            MonthlyBudgetInfo entity = await ctx.MonthlyBudgetInfo.SingleAsync();
            Assert.AreEqual(createMonthlyBudgetInfoModel.Description, entity.Description);
            createMonthlyBudgetInfoModel.Description = "Description2";
            await monthlyBudgetInfoService.UpdateMonthlyBudgetInfoAsync(entity.MonthlyBudgetInfoId,
                createMonthlyBudgetInfoModel, CancellationToken.None);
            entity = await ctx.MonthlyBudgetInfo.SingleAsync();
            Assert.AreEqual(createMonthlyBudgetInfoModel.Description, entity.Description);
        }

        [TestMethod]
        public async Task Test_GetMyMonthlyBudgetInfoListAsync()
        {
            FairPlayBudgetDatabaseContext ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IMonthlyBudgetInfoService monthlyBudgetInfoService = await ServerSideServicesTestBase.GetMonthlyBudgetInfoServiceAsync();
            await ctx.MonthlyBudgetInfo.AddAsync(new MonthlyBudgetInfo()
            {
                Description = "Test 1",
                OwnerId = userEntity.Id,
            });
            await ctx.MonthlyBudgetInfo.AddAsync(new MonthlyBudgetInfo()
            {
                Description = "Test 2",
                OwnerId = userEntity.Id,
            });
            await ctx.MonthlyBudgetInfo.AddAsync(new MonthlyBudgetInfo()
            {
                Description = "Test 3",
                OwnerId = userEntity.Id,
            });
            await ctx.SaveChangesAsync();
            var result = await monthlyBudgetInfoService.GetMyMonthlyBudgetInfoListAsync(CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
        }

        [TestMethod]
        public async Task Test_ImportFromTransactionsFileStreamAsync()
        {
            FairPlayBudgetDatabaseContext ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IMonthlyBudgetInfoService monthlyBudgetInfoService = await ServerSideServicesTestBase.GetMonthlyBudgetInfoServiceAsync();
            var bytes = Encoding.UTF8.GetBytes(Properties.Resources.TransactionsFileTemplate);
            CreateMonthlyBudgetInfoModel? result = null;
            using (MemoryStream memoryStream = new(bytes))
            {
                result = 
                await monthlyBudgetInfoService.ImportFromTransactionsFileStreamAsync(
                    memoryStream, CancellationToken.None);
                memoryStream.Close();
            }
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result!.Transactions!.Count);
        }

        [TestMethod]
        public async Task Test_ImportFromCreditCardFileStreamAsync()
        {
            FairPlayBudgetDatabaseContext ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IMonthlyBudgetInfoService monthlyBudgetInfoService = await ServerSideServicesTestBase.GetMonthlyBudgetInfoServiceAsync();
            var bytes = Encoding.UTF8.GetBytes(Properties.Resources.CreditTransactionsFileTemplate);
            CreateMonthlyBudgetInfoModel? result = null;
            using (MemoryStream memoryStream = new(bytes))
            {
                result =
                await monthlyBudgetInfoService.ImportFromCreditCardFileStreamAsync(
                    memoryStream, CancellationToken.None);
                memoryStream.Close();
            }
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result!.Transactions!.Count);
        }

        [TestMethod]
        public async Task Test_LoadMonthlyBudgetInfoAsync()
        {
            FairPlayBudgetDatabaseContext ctx = await ServerSideServicesTestBase.GetFairPlayBudgetDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IMonthlyBudgetInfoService monthlyBudgetInfoService = await ServerSideServicesTestBase.GetMonthlyBudgetInfoServiceAsync();
            await ctx.MonthlyBudgetInfo.AddAsync(new MonthlyBudgetInfo()
            {
                Description = "Test 1",
                OwnerId = userEntity.Id,
            });
            await ctx.SaveChangesAsync();
            var entity = ctx.MonthlyBudgetInfo.Single();
            var result = await monthlyBudgetInfoService.LoadMonthlyBudgetInfoAsync(entity.MonthlyBudgetInfoId,CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(entity.Description, result.Description);
        }
    }
}
