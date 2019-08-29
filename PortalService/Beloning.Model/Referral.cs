using Beloning.Model.Base;
using Beloning.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Beloning.Model
{
    [Table(TableName)]
    public class Referral : AuditableSoftDeleteEntity
    {
        public const string TableName = "Referrals";

        public ReferralStatus Status { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }
        public int PatientId { get; private set; }
        public Patient Patient { get; private set; }
        private readonly List<ReferralFile> _referralFiles = new List<ReferralFile>();
        public IEnumerable<ReferralFile> ReferralFiles => _referralFiles.ToList();

        public static Referral CreateReferral(int userId, int patientId, ReferralStatus status)
        {
            return new Referral
            {
                UserId = userId,
                PatientId = patientId, 
                Status = status
            };
        }

        public void AddFile(ReferralFile file)
        {
            _referralFiles.Add(file);
        }

        public void RemoveFIle(int id)
        {
            var file = _referralFiles?.FirstOrDefault(p => p.Id == id);
            if (file == null)
            {
                throw new ArgumentException($"No file found with id {id}");
            }
            _referralFiles.Remove(file);
        }

        public void UpdateStatus (ReferralStatus newStatus)
        {
            this.Status = newStatus;
        }

    }
}
