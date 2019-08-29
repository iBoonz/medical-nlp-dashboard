using System;
using System.Threading.Tasks;

namespace Beloning.Data.Repository
{
    public interface IRepository<T> : IDisposable
    {
        Task<T> Get(int id);
        Task<T> Add(T item);
        void Update(T item);
        void Remove(int id);
        void Remove(T entity);
    }
}
