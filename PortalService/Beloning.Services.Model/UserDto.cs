using System;
using System.Collections.Generic;

namespace Beloning.Services.Model
{
    public class UserDto: AuditableEntityDto
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get;  set; }
        public string Name { get;  set; }
        public int Language { get;  set; }
        public int Gender { get; set; }
    }
}
