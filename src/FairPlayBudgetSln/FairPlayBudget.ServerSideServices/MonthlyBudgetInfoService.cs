using CsvHelper;
using CsvHelper.Configuration;
using FairPlayBudget.Common.Enums;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.MonthlyBudgetInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.ServerSideServices
{
    public class MonthlyBudgetInfoService : IMonthlyBudgetInfoService
    {
        private readonly FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext;
        private readonly IUserProviderService userProvider;

        public MonthlyBudgetInfoService(FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext, IUserProviderService userProvider)
        {
            this.fairPlayBudgetDatabaseContext = fairPlayBudgetDatabaseContext;
            this.userProvider = userProvider;
        }

        public async Task CreateMonthlyBudgetInfoAsync(
            CreateMonthlyBudgetInfoModel createMonthlyBudgetInfoModel,
            CancellationToken cancellationToken)
        {
            var userId = this.userProvider.GetCurrentUserId();
            MonthlyBudgetInfo entity = new MonthlyBudgetInfo();
            entity.Description = createMonthlyBudgetInfoModel.Description;
            entity.OwnerId = userId;
            if (createMonthlyBudgetInfoModel.Transactions?.Count > 0)
            {
                foreach (var singleTransaction in createMonthlyBudgetInfoModel.Transactions)
                {
                    switch (singleTransaction.TransactionType)
                    {
                        case Common.Enums.TransactionType.Debit:
                            entity.Expense.Add(new Expense()
                            {
                                Amount = singleTransaction.Amount!.Value,
                                Description = singleTransaction.Description,
                                ExpenseDateTime = singleTransaction.TransactionDateTime!.Value,
                                OwnerId = userId,
                                CurrencyId = (int)singleTransaction.Currency!.Value
                            });
                            break;
                        case Common.Enums.TransactionType.Credit:
                            entity.Income.Add(new Income()
                            {
                                Amount = singleTransaction.Amount!.Value,
                                Description = singleTransaction.Description,
                                IncomeDateTime = singleTransaction.TransactionDateTime!.Value,
                                OwnerId = userId,
                                CurrencyId = (int)singleTransaction.Currency!.Value
                            });
                            break;
                    };
                }
                await this.fairPlayBudgetDatabaseContext.MonthlyBudgetInfo
                    .AddAsync(entity, cancellationToken: cancellationToken);
                await this.fairPlayBudgetDatabaseContext.SaveChangesAsync(
                    cancellationToken: cancellationToken);
            }
        }

        public async Task UpdateMonthlyBudgetInfoAsync(int monthlyBudgetInfoModelId, CreateMonthlyBudgetInfoModel createMonthlyBudgetInfoModel, CancellationToken cancellationToken)
        {
            var userId = this.userProvider.GetCurrentUserId();
            var monthlyInfoEntity = await this.fairPlayBudgetDatabaseContext.MonthlyBudgetInfo
                .Include(p=>p.Expense)
                .Include(p=>p.Income)
                .SingleOrDefaultAsync(p => p.MonthlyBudgetInfoId == monthlyBudgetInfoModelId, cancellationToken);
            if (monthlyInfoEntity != null)
            {
                this.fairPlayBudgetDatabaseContext.Expense.RemoveRange(monthlyInfoEntity.Expense);
                this.fairPlayBudgetDatabaseContext.Income.RemoveRange(monthlyInfoEntity.Income);
                this.fairPlayBudgetDatabaseContext.MonthlyBudgetInfo.Remove(monthlyInfoEntity);
                await this.fairPlayBudgetDatabaseContext.SaveChangesAsync(cancellationToken:cancellationToken);
            }
            MonthlyBudgetInfo entity = new MonthlyBudgetInfo();
            entity.Description = createMonthlyBudgetInfoModel.Description;
            entity.OwnerId = userId;
            if (createMonthlyBudgetInfoModel.Transactions?.Count > 0)
            {
                foreach (var singleTransaction in createMonthlyBudgetInfoModel.Transactions)
                {
                    switch (singleTransaction.TransactionType)
                    {
                        case Common.Enums.TransactionType.Debit:
                            entity.Expense.Add(new Expense()
                            {
                                Amount = singleTransaction.Amount!.Value,
                                Description = singleTransaction.Description,
                                ExpenseDateTime = singleTransaction.TransactionDateTime!.Value,
                                OwnerId = userId,
                                CurrencyId = (int)singleTransaction.Currency!.Value
                            });
                            break;
                        case Common.Enums.TransactionType.Credit:
                            entity.Income.Add(new Income()
                            {
                                Amount = singleTransaction.Amount!.Value,
                                Description = singleTransaction.Description,
                                IncomeDateTime = singleTransaction.TransactionDateTime!.Value,
                                OwnerId = userId,
                                CurrencyId = (int)singleTransaction.Currency!.Value
                            });
                            break;
                    };
                }
                await this.fairPlayBudgetDatabaseContext.MonthlyBudgetInfo
                    .AddAsync(entity, cancellationToken: cancellationToken);
            }
            await this.fairPlayBudgetDatabaseContext.SaveChangesAsync(
                    cancellationToken: cancellationToken);
        }

        public async Task<CreateMonthlyBudgetInfoModel> ImportFromTransactionsFileStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                CreateMonthlyBudgetInfoModel result = new CreateMonthlyBudgetInfoModel()
                {
                    Transactions = new List<CreateTransactionModel>()
                };
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    using (CsvParser csvParser = new CsvParser(streamReader, configuration:
                        new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                        {
                            Delimiter = ";",
                            ShouldQuote = ((ShouldQuoteArgs args) => { return false; })
                        }))
                    {
                        using (CsvReader csvReader = new CsvReader(csvParser))
                        {
                            var records = csvReader.GetRecordsAsync<ImportTransactionsMonthlyBudgetInfoModel>(
                                                        cancellationToken: cancellationToken)
                                                        .ConfigureAwait(continueOnCapturedContext: false);
                            await foreach (var singleRecord in records)
                            {
                                if (String.IsNullOrWhiteSpace(singleRecord.fechaMovimiento))
                                    continue;
                                DateTime dt = DateTime
                                    .ParseExact(singleRecord.fechaMovimiento!, "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture);

                                CreateTransactionModel transaction = new CreateTransactionModel()
                                {
                                    Description = singleRecord.descripcion,
                                    TransactionDateTime = dt,
                                };
                                if (singleRecord.debito != null)
                                {
                                    transaction.Amount = singleRecord.debito;
                                    transaction.TransactionType = Common.Enums.TransactionType.Debit;
                                }
                                else
                                if (singleRecord.credito != null)
                                {
                                    transaction.Amount = singleRecord.credito;
                                    transaction.TransactionType = Common.Enums.TransactionType.Credit;
                                }
                                else
                                {
                                    throw new Exception("There are rows with no value for either debit nor credit");
                                }
                                result.Transactions!.Add(transaction);
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CreateMonthlyBudgetInfoModel> ImportFromCreditCardFileStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                CreateMonthlyBudgetInfoModel result = new CreateMonthlyBudgetInfoModel()
                {
                    Transactions = new List<CreateTransactionModel>()
                };
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    using (CsvParser csvParser = new CsvParser(streamReader, configuration:
                        new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                        {
                            Delimiter = ",",
                            ShouldQuote = ((ShouldQuoteArgs args) => { return false; })
                        }))
                    {
                        using (CsvReader csvReader = new CsvReader(csvParser))
                        {
                            var records = csvReader.GetRecordsAsync<ImportCreditCardMonthlyBudgetInfoModel>(
                                                        cancellationToken: cancellationToken)
                                                        .ConfigureAwait(continueOnCapturedContext: false);
                            await foreach (var singleRecord in records)
                            {
                                if (String.IsNullOrWhiteSpace(singleRecord.Fecha))
                                    continue;
                                try
                                {
                                    DateTime dt = DateTime
                                        .ParseExact(singleRecord.Fecha!, "d/M/yyyy",
                                        CultureInfo.InvariantCulture);

                                    CreateTransactionModel transaction = new CreateTransactionModel()
                                    {
                                        Description = singleRecord.Establecimiento,
                                        TransactionDateTime = dt,
                                    };
                                    if (singleRecord.Monto != null)
                                    {
                                        if (singleRecord.Monto.Value > 0)
                                        {
                                            transaction.Amount = singleRecord.Monto;
                                            transaction.TransactionType = Common.Enums.TransactionType.Debit;
                                        }
                                        else
                                        {
                                            transaction.Amount = singleRecord.Monto;
                                            transaction.TransactionType = Common.Enums.TransactionType.Credit;
                                        }

                                    }
                                    else
                                    {
                                        throw new Exception("There are rows with no value for either debit nor credit");
                                    }
                                    switch (singleRecord.Moneda)
                                    {
                                        case "Colones":
                                            transaction.Currency = Common.Enums.Currency.CRC;
                                            break;
                                        case "Dólares":
                                            transaction.Currency = Common.Enums.Currency.USD;
                                            break;
                                    }
                                    result.Transactions!.Add(transaction);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CreateMonthlyBudgetInfoModel> LoadMonthlyBudgetInfoAsync(int monthlyBudgetInfoId, CancellationToken cancellationToken)
        {
            var entity = await this.fairPlayBudgetDatabaseContext.MonthlyBudgetInfo
                .Include(p => p.Expense)
                .Include(p => p.Income)
                .Where(p => p.MonthlyBudgetInfoId == monthlyBudgetInfoId)
                .SingleAsync(cancellationToken:cancellationToken);
            CreateMonthlyBudgetInfoModel result = new CreateMonthlyBudgetInfoModel();
            result.Description = entity.Description;
            result.Transactions =
                    entity.Income.Select(p => new CreateTransactionModel()
                    {
                        Amount = p.Amount,
                        Currency = Enum.Parse<Common.Enums.Currency>(p.CurrencyId.ToString()),
                        Description = p.Description,
                        TransactionDateTime = p.IncomeDateTime,
                        TransactionType = TransactionType.Credit
                    })
                    .Union(entity.Expense.Select(p => new CreateTransactionModel()
                    {
                        Amount = p.Amount,
                        Currency = Enum.Parse<Common.Enums.Currency>(p.CurrencyId.ToString()),
                        Description = p.Description,
                        TransactionDateTime = p.ExpenseDateTime,
                        TransactionType = TransactionType.Debit
                    }))
                    .OrderByDescending(p => p.TransactionDateTime).ToList();
            return result;
        }

        public async Task<MonthlyBudgetInfoModel[]?> GetMyMonthlyBudgetInfoListAsync(CancellationToken cancellationToken)
        {
            var userId = this.userProvider.GetCurrentUserId();
            var result = await this.fairPlayBudgetDatabaseContext.MonthlyBudgetInfo.AsNoTracking()
                .Where(p => p.OwnerId == userId)
                .Select(p=> new MonthlyBudgetInfoModel()
                {
                    MonthlyBudgetInfoId = p.MonthlyBudgetInfoId,
                    Description = p.Description
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
