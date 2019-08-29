using System;
using System.ComponentModel.DataAnnotations;

namespace Beloning.Model.Contracts
{
    public interface IAuditableEntity : IEntity
    {
        [Required, Display(AutoGenerateField = false)]
        int ModifiedBy { get; set; }

        [Required, Display(AutoGenerateField = false)]
        int CreatedBy { get; set; }
 
        [Required, Display(AutoGenerateField = false)]
        DateTime ModifiedOn { get; set; }

        [Required, Display(AutoGenerateField = false)]
        DateTime CreatedOn { get; set; }
  
    }
}