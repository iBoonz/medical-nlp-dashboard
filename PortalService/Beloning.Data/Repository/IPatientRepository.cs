using System.Collections.Generic;
using System.Threading.Tasks;
using Beloning.Model;

namespace Beloning.Data.Repository
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<List<Patient>> Find(string name, int skip, int take);
        Task<Patient> GetByNihii(string email);

    }
}
