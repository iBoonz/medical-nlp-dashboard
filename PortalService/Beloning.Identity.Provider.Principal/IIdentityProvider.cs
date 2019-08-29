namespace Beloning.Identity.Provider.Principal
{
    public interface IIdentityProvider
    {
        string SubjectId { get; }
        int UserId { get; }
    }
}
