using FairPlayBudget.Interfaces.Services;

namespace FairPlayBudget.Services
{
    public class UserProviderService(IHttpContextAccessor httpContextAccessor) : IUserProviderService
    {
        public string GetCurrentUserId()
        {
            var claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            var id = httpContextAccessor!.HttpContext!.User.Claims.Single(p => p.Type == claimType)!.Value!;
            return id;
        }
    }
}
