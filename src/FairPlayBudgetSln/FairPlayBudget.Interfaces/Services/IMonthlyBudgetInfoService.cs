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
    }
}
