﻿using FairPlayBudget.DataAccess.Data;
using FairPlayBudget.DataAccess.Models;
using FairPlayBudget.Interfaces.Services;
using FairPlayBudget.Models.MonthlyBudgetInfo;
using System;
using System.Collections.Generic;
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
                                AmountInUsd=singleTransaction.AmountInUsd!.Value,
                                Description=singleTransaction.Description,
                                ExpenseDateTime=singleTransaction.TransactionDateTime!.Value,
                                OwnerId=userId
                            });
                            break;
                        case Common.Enums.TransactionType.Credit:
                            entity.Income.Add(new Income()
                            {
                                AmountInUsd = singleTransaction.AmountInUsd!.Value,
                                Description = singleTransaction.Description,
                                IncomeDateTime = singleTransaction.TransactionDateTime!.Value,
                                OwnerId = userId
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
    }
}
