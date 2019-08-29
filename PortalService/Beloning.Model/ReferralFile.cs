using Beloning.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Beloning.Model
{
    [Table(TableName)]
    public class ReferralFile : Entity
    {
        public const string TableName = "ReferralFiles";

        public int ReferralId { get; private set; }
        public Referral Referral { get; private set; }
        public string FileName { get; private set; }

        public static ReferralFile CreateReferralFile(int referralId, string fileName)
        {
            return new ReferralFile
            {
                ReferralId = referralId,
                FileName = fileName
            };
        }
    }
}
