using System.Security.Claims;

namespace Beloning.Identity.Provider.Principal
{
    public class IdentityProviderMock : IIdentityProvider
    {
        private readonly ClaimsIdentity _claimsIdentity;

        public int UserId
        {
            get
            {
                return 999;
            }
        }

       
        public string SubjectId
        {
            get
            {
                return "mock";
            }
        }
    }
}
