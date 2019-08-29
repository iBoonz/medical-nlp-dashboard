using System.ComponentModel.DataAnnotations;

namespace Beloning.Model.Contracts
{
    public interface IOrdered
    {
        [Required]
        int Order { get; }
    }
}