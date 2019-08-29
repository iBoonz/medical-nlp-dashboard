using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beloning.Data.Repository;
using Beloning.Model;
using Beloning.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace Beloning.Data.Sql.Repositories
{
    public class ReferralRepository : Repository<Referral>, IReferralRepository
    {
        public ReferralRepository(BeloningContext context) : base(context)
        {
        }
   

        public async Task<List<Referral>> Find(int skip, int take)
        {
            var referrals = Context.Referrals.
                            Include(p => p.Patient).
                            Include(p => p.User)
                           .Where(p => !p.IsDeleted);
           
            return await referrals.OrderBy(p => p.CreatedOn).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Referral> Update(int id, ReferralStatus status)
        {
            var referral = await Context.Referrals.FirstAsync(p => p.Id == id);
            referral.UpdateStatus(status);
            return referral;
        }
    }
}
