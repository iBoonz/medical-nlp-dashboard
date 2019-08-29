using System;

namespace Beloning.Model.Contracts
{
    public interface IEntityWithStamp : IEntity
    {
        DateTime CreatedOn { get; set; }
    }
}