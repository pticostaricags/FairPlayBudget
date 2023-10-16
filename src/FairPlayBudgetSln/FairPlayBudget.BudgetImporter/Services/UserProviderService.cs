using FairPlayBudget.BudgetImporter.Configuration;
using FairPlayBudget.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.BudgetImporter.Services
{
    public class UserProviderService : IUserProviderService
    {
        private readonly ImportConfiguration importConfiguration;

        public UserProviderService(ImportConfiguration importConfiguration) 
        {
            this.importConfiguration = importConfiguration;
        }
        public string GetCurrentUserId()
        {
            return this.importConfiguration.UserId!;
        }
    }
}
