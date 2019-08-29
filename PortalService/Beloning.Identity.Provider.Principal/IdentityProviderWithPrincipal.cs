using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beloning.Identity.Provider.Principal
{
    public class IdentityProviderWithPrincipal : IIdentityProvider
    {
        private readonly ClaimsIdentity _claimsIdentity;

        public IdentityProviderWithPrincipal(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
                _claimsIdentity = (ClaimsIdentity)httpContextAccessor.HttpContext.User.Identity;
        }

        public int UserId
        {
            get
            {
                var claim = _claimsIdentity.FindFirst("UserId");
                return int.Parse(claim.Value);
            }
        }

       
        public string SubjectId
        {
            get
            {
                var claim = _claimsIdentity.FindFirst("sub");
                return claim.Value;
            }
        }
    }
}
