using FairPlayBudget.BudgetImporter.Configuration;
using FairPlayBudget.Common.Enums;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.ServerSideServices;
using Microsoft.EntityFrameworkCore;

namespace FairPlayBudget.BudgetImporter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly FairPlayBudgetDatabaseContext _fairPlayBudgetDatabaseContext;
        private readonly ImportConfiguration _importConfiguration;
        private readonly MonthlyBudgetInfoService _monthlyBudgetInfoService;

        public Worker(ILogger<Worker> logger, 
            FairPlayBudgetDatabaseContext fairPlayBudgetDatabaseContext,
            ImportConfiguration importConfiguration,
            FairPlayBudget.ServerSideServices.MonthlyBudgetInfoService monthlyBudgetInfoService
            )
        {
            _logger = logger;
            _fairPlayBudgetDatabaseContext = fairPlayBudgetDatabaseContext;
            _importConfiguration = importConfiguration;
            _monthlyBudgetInfoService = monthlyBudgetInfoService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    foreach (var yearDirectory in Directory.GetDirectories(this._importConfiguration.ImportFolder!))
                    {
                        foreach (var monthDirectory in Directory.GetDirectories(yearDirectory))
                        {
                            foreach (var currencyDirectory in Directory.GetDirectories(monthDirectory))
                            {
                                var currencyCode = Path.GetFileName(currencyDirectory);
                                foreach (var importFile in Directory.GetFiles(currencyDirectory, "*.csv"))
                                {
                                    using (Stream streamReader = File.Open(importFile, FileMode.Open))
                                    {
                                        var importedData =
                                            await this._monthlyBudgetInfoService
                                            .ImportFromFileStreamAsync(streamReader, stoppingToken);
                                        importedData.Transactions!.AsParallel().ForAll(p => 
                                        {
                                            p.Currency = Enum.Parse<Currency>(currencyCode!);
                                        });
                                        importedData.Description = Path.GetFileName(importFile);
                                        await this._monthlyBudgetInfoService!
                                            .CreateMonthlyBudgetInfoAsync(importedData, stoppingToken);
                                        streamReader.Close();
                                    }
                                }
                            }
                        }
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
