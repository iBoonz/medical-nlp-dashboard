using System;

namespace Beloning.Services.Model
{
    public class AuditableEntityDto: EntityDto
    {
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
    }
}