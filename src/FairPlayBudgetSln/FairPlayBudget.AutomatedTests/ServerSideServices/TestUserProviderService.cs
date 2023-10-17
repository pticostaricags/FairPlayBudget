using FairPlayBudget.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.AutomatedTests.ServerSideServices
{
    public class TestUserProviderService : IUserProviderService
    {
        public string GetCurrentUserId()
        {
            return ServerSideServicesTestBase.CurrentUserId!;
        }
    }
}
