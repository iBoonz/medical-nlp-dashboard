using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Beloning.Model.Base
{
    public abstract class Entity 
    { 
        [Display(AutoGenerateField = true), Key, Required, Column(Order = 0)]
        public int Id { get; set; }
    }
}