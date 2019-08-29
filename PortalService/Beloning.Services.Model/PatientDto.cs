using System;

namespace Beloning.Services.Model
{
    public class PatientDto : UserDto
    {
        public DateTime DateOfBirth { get; set; }
        public string Remarks { get; set; }
        public string Nihii { get; set; }
    }
}
