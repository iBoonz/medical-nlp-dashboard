using Beloning.Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Beloning.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IPatientRepository PatientRepository { get; }
        IReferralRepository ReferralRepository { get; }
        int SaveChanges();

    }
}
