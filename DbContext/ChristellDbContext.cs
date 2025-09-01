using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MobileNumLogin.Models;

public partial class ChristellDbContext : DbContext
{
    public ChristellDbContext()
    {
    }

    public ChristellDbContext(DbContextOptions<ChristellDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcrmcustomerInfo> AcrmcustomerInfos { get; set; }

    public virtual DbSet<SystemAutoNo> SystemAutoNos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ftps14000.applyfree.cloud,14033;Database=ChristellOffice;User Id=xenosys;Password=Cha@123;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcrmcustomerInfo>(entity =>
        {
            entity.HasKey(e => e.RegNo);

            entity.ToTable("ACRMCustomerInfo");

            entity.HasIndex(e => new { e.MobileNo, e.Fname }, "IX_ACRMCustomerMobileNo");

            entity.Property(e => e.RegNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.Age).HasDefaultValue(0);
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CustomerType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DoB).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EnteredDatetime).HasColumnType("datetime");
            entity.Property(e => e.Fname)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("FName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HomeCity)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.HomePhone)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasDefaultValueSql("((0))")
                .IsFixedLength();
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsRegular).HasDefaultValue(false);
            entity.Property(e => e.IsVip)
                .HasDefaultValue(false)
                .HasColumnName("IsVIP");
            entity.Property(e => e.IsWalking).HasDefaultValue(true);
            entity.Property(e => e.Lname)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("LName");
            entity.Property(e => e.LoyaltyCardNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MobileNo)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ModifiedDatetime).HasColumnType("datetime");
            entity.Property(e => e.Nic).HasColumnName("NIC");
            entity.Property(e => e.PassportNo).HasMaxLength(50);
            entity.Property(e => e.Profession)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.RefereId).HasColumnName("RefereID");
            entity.Property(e => e.RegDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Salutation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SignatureUrl).HasColumnName("SignatureURL");
            entity.Property(e => e.StatusRemark)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SysDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<SystemAutoNo>(entity =>
        {
            entity.HasKey(e => e.FormType);

            entity.ToTable("SystemAutoNo");

            entity.HasIndex(e => e.LastModifiedDate, "IX_SystemAutoNo");

            entity.Property(e => e.FormType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsDateUpdated).HasDefaultValue(false);
            entity.Property(e => e.IsEveningUpdated).HasDefaultValue(false);
            entity.Property(e => e.IsMorningUpdated).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
