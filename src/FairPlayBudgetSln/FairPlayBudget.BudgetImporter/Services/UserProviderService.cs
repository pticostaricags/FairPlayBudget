using FairPlayBudget.BudgetImporter.Configuration;
using FairPlayBudget.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.BudgetImporter.Services
{
    public class UserProviderService(ImportConfiguration importConfiguration) : IUserProviderService
    {
        public string GetCurrentUserId()
        {
            return importConfiguration.UserId!;
        }
    }
}
