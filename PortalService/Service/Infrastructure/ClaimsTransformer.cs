using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Service.Infrastructure
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly IPrincipal _principal;
        private IMemoryCache _cache;

        public ClaimsTransformer(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _principal = httpContextAccessor.HttpContext.User;
            _cache = memoryCache;
        }
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var currentPrincipal = (ClaimsIdentity)_principal.Identity;
            var identity = (ClaimsIdentity)principal.Identity;
            if (currentPrincipal.Claims.All(p => p.Type != "UserId"))
            {
                //Person person;
                //var sub = principal.Claims.First(p => p.Type == "sub").Value;
                //if (!_cache.TryGetValue(sub, out person))
                //{
                //    var personBySubj = _unitOfWork.PersonRepository.GetPersonBySubjectId(sub);
                //    personBySubj.Wait();
                //    person = personBySubj.Result;
                //    if (person != null)
                //    {
                //        // Set cache options.
                //        var cacheEntryOptions = new MemoryCacheEntryOptions()
                //            // Keep in cache for this time, reset time if accessed.
                //            .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                //        // Save data in cache.
                //        _cache.Set(sub, person, cacheEntryOptions);
                //    }
                //}

                //if (person != null)
                //{
                //    currentPrincipal.AddClaim(new Claim("UserId", person.Id.ToString()));
                //    currentPrincipal.AddClaim(new Claim("TenantId", person.PersonTeams.FirstOrDefault(p => p.Team.TeamType == TeamType.OrganizationTeam)?.Team.OrganizationId.ToString()));
                //    if (principal.Claims.Any(p => p.Type == "Admin"))
                //    {
                //        currentPrincipal.AddClaim(new Claim("Admin", "True"));
                //    }
                //}
                foreach (var claim in identity.Claims)
                {
                    currentPrincipal.AddClaim(claim);
                }
            }
            return Task.FromResult(principal);
        }
    }
}
