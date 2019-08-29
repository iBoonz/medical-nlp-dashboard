using System.Collections.Generic;
using System.Threading.Tasks;
using Beloning.Model;
using Beloning.Model.Enum;

namespace Beloning.Data.Repository
{
    public interface IReferralRepository : IRepository<Referral>
    {
        Task<List<Referral>> Find(int skip, int take);
        Task<Referral> Update(int id, ReferralStatus status);

    }
}
