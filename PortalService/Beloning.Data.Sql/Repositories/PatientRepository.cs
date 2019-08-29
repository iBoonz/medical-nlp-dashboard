using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beloning.Data.Repository;
using Beloning.Model;
using Microsoft.EntityFrameworkCore;

namespace Beloning.Data.Sql.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(BeloningContext context) : base(context)
        {
        }
   

        public async Task<List<Patient>> Find(string name, int skip, int take)
        {
            var patients = Context.Patients
                .Where(p => !p.IsDeleted);
            if (!string.IsNullOrWhiteSpace(name))
            {
                patients = patients.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }
            return await patients.OrderBy(p => p.CreatedOn).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Patient> GetByNihii(string nihii)
        {
            return await Context.Patients
                .Where(p => p.Nihii == nihii)
                .FirstOrDefaultAsync();
        }

    }
}
