using Beloning.Model.Base;
using Beloning.Model.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Beloning.Model
{
    [Table(TableName)]
    public class User: AuditableSoftDeleteEntity
    {
        public const string TableName = "Users";
        public string Email { get; protected set; }
        public string Name { get; protected set; }
        public Language Language { get; protected set; }
      

        public static User CreateUser(string email, string name, Language language)
        {
            return new User
            {
                Email = email,
                Name = name,
                Language = language,
            };
        }

    }
}
