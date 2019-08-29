using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Beloning.Model.Contracts;

namespace Beloning.Model.Base
{
    public abstract class AuditableEntity: Entity, IAuditableEntity
    {
        [Required, Display(AutoGenerateField = false), Column(Order = 1)]
        public DateTime CreatedOn { get; set; }

        [Required, Display(AutoGenerateField = false), Column(Order = 2)]
        public int CreatedBy { get; set; }

        [Required, Display(AutoGenerateField = false), Column(Order = 3)]
        public DateTime ModifiedOn { get; set; }

        [Required, Display(AutoGenerateField = false), Column(Order = 4)]
        public int ModifiedBy { get; set; }
    }

}