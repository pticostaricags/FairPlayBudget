using FairPlayBudget.BudgetImporter;
using FairPlayBudget.BudgetImporter.Configuration;
using FairPlayBudget.BudgetImporter.Services;
using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.ServerSideServices;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
ImportConfiguration importConfiguration = new ImportConfiguration();
importConfiguration.ImportFolder = builder.Configuration["ImportFolder"];
importConfiguration.UserId = builder.Configuration["UserId"];
builder.Services.AddSingleton<ImportConfiguration>(importConfiguration);
builder.Services.AddSingleton<MonthlyBudgetInfoService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<FairPlayBudgetDatabaseContext>(
    optionsAction =>
    {
        optionsAction.UseSqlServer(
            connectionString, sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null);
            });
    }, contextLifetime: ServiceLifetime.Singleton,
    optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddSingleton<IUserProviderService, UserProviderService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
