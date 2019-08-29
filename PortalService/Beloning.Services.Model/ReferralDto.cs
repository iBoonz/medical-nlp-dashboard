namespace Beloning.Services.Model
{
    public class ReferralDto : AuditableEntityDto
    {
        public string[] FileNames { get; set; }
        public int Status { get; set; }
        public PatientDto Patient { get; set; }
        public UserDto User { get; set; }
    }
}
