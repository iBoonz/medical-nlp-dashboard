using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beloning.Identity.Provider.Principal
{
    public class IdentityProviderFunction : IIdentityProvider
    {
        private readonly ClaimsIdentity _claimsIdentity;

        public int UserId
        {
            get
            {
                return 100000;
            }
        }

       
        public string SubjectId
        {
            get
            {
                return "Function";
            }
        }
    }
}
