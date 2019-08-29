namespace Beloning.Model.Contracts
{
    public interface ISoftDeleteEntity
    {
        bool IsDeleted { get; set; }
    }
}