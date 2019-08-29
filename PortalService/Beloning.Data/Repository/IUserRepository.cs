using Beloning.Model;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Beloning.Data.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> Find(string name, int skip, int take);
        Task<User> Get(string email);
        Task<User> GetByMail(string email);
    }
}
