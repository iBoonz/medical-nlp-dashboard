﻿// <auto-generated />
using System;
using Beloning.Data.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beloning.Data.Sql.Migrations
{
    [DbContext(typeof(BeloningContext))]
    partial class BeloningContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Beloning.Model.Referral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ModifiedBy");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int>("PatientId");

                    b.Property<int>("Status");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.HasIndex("UserId");

                    b.ToTable("Referrals");
                });

            modelBuilder.Entity("Beloning.Model.ReferralFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FileName");

                    b.Property<int>("ReferralId");

                    b.HasKey("Id");

                    b.HasIndex("ReferralId");

                    b.ToTable("ReferralFiles");
                });

            modelBuilder.Entity("Beloning.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("Language");

                    b.Property<int>("ModifiedBy");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("Beloning.Model.Patient", b =>
                {
                    b.HasBaseType("Beloning.Model.User");

                    b.Property<Guid>("AnonymizationId");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<int>("Gender");

                    b.Property<string>("Nihii");

                    b.Property<string>("Remarks");

                    b.ToTable("Patients");

                    b.HasDiscriminator().HasValue("Patient");
                });

            modelBuilder.Entity("Beloning.Model.Referral", b =>
                {
                    b.HasOne("Beloning.Model.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Beloning.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Beloning.Model.ReferralFile", b =>
                {
                    b.HasOne("Beloning.Model.Referral", "Referral")
                        .WithMany("ReferralFiles")
                        .HasForeignKey("ReferralId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
