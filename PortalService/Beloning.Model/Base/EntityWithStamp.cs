using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Beloning.Model.Contracts;

namespace Beloning.Model.Base
{
    public abstract class EntityWithStamp : Entity, IEntityWithStamp
    {
        [Display(AutoGenerateField = true), Required, Column(Order = 1)]
        public DateTime CreatedOn { get; set; }
    }
}