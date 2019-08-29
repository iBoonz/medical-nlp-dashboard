using System.Collections.Generic;

namespace Beloning.Services.Model
{
    public class DashboardDto: EntityDto
    {
        public int TotalReferrals { get; set; }
        public int ResolvedReferrals { get; set; }
        public int OpenReferrals { get; set; }
        public int DeniedReferrals { get; set; }
        public int FemaleRatio { get; set; }
        public int MaleRatio { get; set; }
        public List<ReferralInfo> ReferralInfo { get; set; }

    }
}
