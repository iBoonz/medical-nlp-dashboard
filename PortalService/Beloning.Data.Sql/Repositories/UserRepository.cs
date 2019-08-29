using Beloning.Data.Repository;
using Beloning.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beloning.Data.Sql.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BeloningContext context) : base(context)
        {
        }

        public async Task<User> Get(string email)
        {
            return await Context.Users
                .Where(p => p.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> Find(string name, int skip, int take)
        {
            var users = Context.Users.Where(p => !p.IsDeleted);
            if (!string.IsNullOrWhiteSpace(name))
            {
                users = users.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }
            return await users.OrderBy(p => p.CreatedOn).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<User> GetByMail(string email)
        {
            return await Context.Users
                .Where(p => p.Email == email)
                .FirstOrDefaultAsync();
        }

        public override async Task<User> Get(int userId)
        {
            return await Context.Users
                .Where(p => p.Id == userId)
                .FirstOrDefaultAsync();
        }

    }
}
