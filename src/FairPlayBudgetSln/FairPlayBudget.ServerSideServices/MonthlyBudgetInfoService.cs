using CsvHelper;
using CsvHelper.Configuration;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.MonthlyBudgetInfo;
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

        public async Task<CreateMonthlyBudgetInfoModel> ImportFromFileStreamAsync(Stream stream, CancellationToken cancellationToken)
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
                            var records = csvReader.GetRecordsAsync<ImportMonthlyBudgetInfoModel>(
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
    }
}
