using FairPlayBudget.BudgetImporter.Configuration;
using FairPlayBudget.Common.Enums;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.Models.MonthlyBudgetInfo;
using FairPlayBudget.ServerSideServices;
using Microsoft.EntityFrameworkCore;

namespace FairPlayBudget.BudgetImporter
{
    public class Worker(ILogger<Worker> logger,
        ImportConfiguration importConfiguration,
        MonthlyBudgetInfoService monthlyBudgetInfoService) : BackgroundService
    {
        private readonly ImportConfiguration _importConfiguration = importConfiguration;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    foreach (var yearDirectory in Directory.GetDirectories(this._importConfiguration.ImportFolder!))
                    {
                        foreach (var monthDirectory in Directory.GetDirectories(yearDirectory))
                        {
                            foreach (var currencyDirectory in Directory.GetDirectories(monthDirectory))
                            {
                                var currencyCode = Path.GetFileName(currencyDirectory);
                                foreach (var importFile in Directory.GetFiles(currencyDirectory, "*.csv"))
                                {
                                    using Stream streamReader = File.Open(importFile, FileMode.Open);
                                    CreateMonthlyBudgetInfoModel? importedData = null;
                                    switch (currencyCode)
                                    {
                                        case "Credit":
                                            importedData =
                                        await monthlyBudgetInfoService
                                        .ImportFromCreditCardFileStreamAsync(streamReader, stoppingToken);
                                            break;
                                        default:
                                            importedData =
                                        await monthlyBudgetInfoService
                                        .ImportFromTransactionsFileStreamAsync(streamReader, stoppingToken);
                                            importedData!.Transactions!.AsParallel().ForAll(p =>
                                            {
                                                p.Currency = Enum.Parse<Currency>(currencyCode!);
                                            });
                                            break;
                                    }
                                    importedData.Description = Path.GetFileName(importFile);
                                    await monthlyBudgetInfoService!
                                        .CreateMonthlyBudgetInfoAsync(importedData, stoppingToken);
                                    streamReader.Close();
                                }
                            }
                        }
                    }
                    logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
