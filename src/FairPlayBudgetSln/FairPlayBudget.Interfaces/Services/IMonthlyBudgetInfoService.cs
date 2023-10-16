using FairPlayBudget.Models.MonthlyBudgetInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Interfaces.Services
{
    public interface IMonthlyBudgetInfoService
    {
        Task CreateMonthlyBudgetInfoAsync(CreateMonthlyBudgetInfoModel createMonthlyBudgetInfoModel,
            CancellationToken cancellationToken);
        Task UpdateMonthlyBudgetInfoAsync(int monthlyBudgetInfoModelId, CreateMonthlyBudgetInfoModel createMonthlyBudgetInfoModel,
            CancellationToken cancellationToken);
        Task<CreateMonthlyBudgetInfoModel> LoadMonthlyBudgetInfoAsync(
            int monthlyBudgetInfoId,
            CancellationToken cancellationToken);
        Task<CreateMonthlyBudgetInfoModel> ImportFromTransactionsFileStreamAsync(Stream stream, CancellationToken cancellationToken);
        Task<CreateMonthlyBudgetInfoModel> ImportFromCreditCardFileStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}
