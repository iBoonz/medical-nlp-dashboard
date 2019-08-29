using Beloning.Data.Repository;
using Beloning.Data.Sql.Repositories;
using Beloning.Data.UnitOfWork;
using Beloning.Identity.Provider.Principal;
using System;

namespace Beloning.Data.Sql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BeloningContext _context;
        private IUserRepository _userRepository;
        private IPatientRepository _patientRepository;
        private IReferralRepository _referralRepostory;

        public IIdentityProvider IdentityProvider { get; }
        public UnitOfWork(BeloningContext context)
        {
            _context = context;
            IdentityProvider = context.IdentityProvider;
        }


        public IUserRepository UserRepository => _userRepository ??
                                                        (_userRepository = new UserRepository(_context));

        public IPatientRepository PatientRepository => _patientRepository ??
                                                      (_patientRepository = new PatientRepository(_context));

        public IReferralRepository ReferralRepository => _referralRepostory ??
                                              (_referralRepostory = new ReferralRepository(_context));

        public int SaveChanges()
        {
            return _context.SaveChanges();
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
                _patientRepository?.Dispose();
                _referralRepostory?.Dispose();
                _userRepository?.Dispose();

            }
        }
    }
}
