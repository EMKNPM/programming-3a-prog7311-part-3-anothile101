using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PracticeAssignment.Models;
using System.Reflection.Emit;


   
 namespace PracticeAssignment.Data;


    public class ApplicationDbContext : DbContext
    {
  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client configuration
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.ContactDetails)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.Region)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasMany(c => c.Contracts)
                      .WithOne(c => c.Client)
                      .HasForeignKey(c => c.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        // Contract configuration
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.ClientId, e.Status });
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);

            entity.Property(e => e.StartDate)
                  .IsRequired();

            entity.Property(e => e.EndDate)
                  .IsRequired();

            entity.Property(e => e.ServiceLevel)
                  .IsRequired()
                  .HasMaxLength(200);

           
            entity.Property(e => e.Status)
                  .HasConversion<string>()
                  .IsRequired();

            entity.Property(e => e.SignedAgreementPath)
                  .HasMaxLength(500);

            entity.Property(e => e.SignedAgreementFileName)
                  .HasMaxLength(255);

            entity.HasMany(e => e.ServiceRequests)
                  .WithOne(e => e.Contract)
                  .HasForeignKey(e => e.ContractId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                  .IsRequired(false);
        });

        // ServiceRequest configuration
        modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.ContractId);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(e => e.CostUsd)
                      .IsRequired()
                      .HasPrecision(18, 2);

                entity.Property(e => e.CostZar)
                      .HasPrecision(18, 2);

                entity.Property(e => e.ExchangeRateUsed)
                      .HasPrecision(18, 6);

                entity.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();
            });
        }
    }
