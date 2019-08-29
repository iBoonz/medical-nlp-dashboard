using Beloning.Model.Enum;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Beloning.Model
{
    [Table(TableName)]
    public class Patient : User
    {
        public const string TableName = "Patients";

        public string Remarks { get; private set; }
        public Gender Gender { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Nihii { get; private set; }
        public Guid AnonymizationId { get; private set; }

        public static Patient CreatePatient(string email, string nihii, string name, 
            Language language, Gender gender, DateTime dateofBirth, string remarks)
        {
            return new Patient
            {
                Email = email,
                Nihii = nihii,
                Name = name,
                Language = language,
                Gender = gender,
                DateOfBirth = dateofBirth,
                Remarks = remarks,
                AnonymizationId = Guid.NewGuid()
            };
        }

    }
}
