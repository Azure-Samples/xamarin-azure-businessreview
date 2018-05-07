using System;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
namespace Reviewer.Core
{
    public interface IIdentityService
    {
        string DisplayName { get; set; }

        Task<AuthenticationResult> Login();
        Task<AuthenticationResult> GetCachedSignInToken();
        void Logout();
        UIParent UIParent { get; set; }
    }
}
