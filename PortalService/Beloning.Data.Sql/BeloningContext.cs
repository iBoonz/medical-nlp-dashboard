using Beloning.Data.Repository;
using Beloning.Identity.Provider.Principal;
using Beloning.Model;
using Beloning.Model.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace Beloning.Data.Sql
{
    public class BeloningContext : DbContext
    {
        public readonly IIdentityProvider IdentityProvider;
        public BeloningContext()
        {
        }


        public BeloningContext(IIdentityProvider identityProvider) 
        {
            IdentityProvider = identityProvider;
        }

        public BeloningContext(DbContextOptions options, IIdentityProvider identityProvider) : base(options)
        {
            IdentityProvider = identityProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<IEntity>().ToList();


            foreach (var entry in entries)
            {
                if (EntryNeedsCheckup(entry))
                {
                    CheckSoftDelete(entry);
                    SetDateTimeForEntityWithTimeStamp(entry);
                    SetAuditableEntityInfo(entry);

                }
            }

            return base.SaveChanges(); 
        }



        private bool EntryNeedsCheckup(EntityEntry entry)
        {
            return entry.State != EntityState.Unchanged;
        }

        private void CheckSoftDelete(EntityEntry entry)
        {
            if (entry.State == EntityState.Deleted)
            {
                var softDeleteEntity = entry.Entity as ISoftDeleteEntity;
                if (softDeleteEntity != null)
                {
                    entry.State = EntityState.Modified;
                    softDeleteEntity.IsDeleted = true;
                }
            }
        }

        private void SetDateTimeForEntityWithTimeStamp(EntityEntry entry)
        {
            var entityWithTimeStamp = entry.Entity as IEntityWithStamp;
            if (entityWithTimeStamp != null)
            {
                entityWithTimeStamp.CreatedOn = DateTime.Now;
            }
        }

        private void SetAuditableEntityInfo(EntityEntry entry)
        {
            var auditableEntity = entry.Entity as IAuditableEntity;
            if (auditableEntity != null)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        auditableEntity.ModifiedOn = DateTime.Now;
                        auditableEntity.ModifiedBy = IdentityProvider.UserId;

                        break;
                    case EntityState.Added:
                        auditableEntity.CreatedOn = DateTime.Now;
                        auditableEntity.ModifiedOn = DateTime.Now;
                        auditableEntity.CreatedBy = IdentityProvider.UserId;
                        auditableEntity.ModifiedBy = IdentityProvider.UserId;
                        break;
                }
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<ReferralFile> ReferralFiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userNotificationsFrom = modelBuilder.Entity<Referral>().Metadata.FindNavigation(nameof(Referral.ReferralFiles));
            userNotificationsFrom.SetField("_referralFiles");
            userNotificationsFrom.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        }
    }
