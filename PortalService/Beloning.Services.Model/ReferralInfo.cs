using System;

namespace Beloning.Services.Model
{
    public class ReferralInfo:EntityDto
    {
        public string PatientName { get; set; }
        public string PhysicianName { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Status { get; set; }
    }
}
