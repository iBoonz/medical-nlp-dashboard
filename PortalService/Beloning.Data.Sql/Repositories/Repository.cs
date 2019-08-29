using Beloning.Data.Repository;
using Beloning.Model.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Beloning.Data.Sql.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : Entity
    {
        protected Repository(BeloningContext context)
        {
            Context = context;
        }

        protected BeloningContext Context { get; }

        protected IQueryable<T> Query
        {
            get
            {
                return Context.Set<T>();
            }
        }


        public async Task<T> Add(T entity)
        {
            return (await Context.Set<T>().AddAsync(entity)).Entity;
        }

        public void Update(T entity)
        {
            Context.Update<T>(entity);
        }

        public virtual async Task<T> Get(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public void Remove(int id)
        {
            Remove(Get(id).Result);
        }

        public virtual void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }
    }
}
