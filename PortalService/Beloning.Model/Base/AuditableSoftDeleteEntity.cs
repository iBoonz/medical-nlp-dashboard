using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Beloning.Model.Contracts;

namespace Beloning.Model.Base
{
    public abstract class AuditableSoftDeleteEntity : AuditableEntity, ISoftDeleteEntity
    {

        [Required, Display(AutoGenerateField = false), Column(Order = 5)]
        public bool IsDeleted { get; set; }
    }
}