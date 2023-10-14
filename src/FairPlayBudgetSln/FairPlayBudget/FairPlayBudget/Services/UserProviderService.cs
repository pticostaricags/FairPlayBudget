using FairPlayBudget.Interfaces.Services;

namespace FairPlayBudget.Services
{
    public class UserProviderService : IUserProviderService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserProviderService(IHttpContextAccessor httpContextAccessor) 
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentUserId()
        {
            var claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            var id = this.httpContextAccessor!.HttpContext!.User.Claims.Single(p => p.Type == claimType)!.Value!;
            return id;
        }
    }
}
