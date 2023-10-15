﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using FairPlayBudget.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace FairPlayBudget.DataAccess.Data;

public partial class FairPlayBudgetDatabaseContext : DbContext
{
    public FairPlayBudgetDatabaseContext(DbContextOptions<FairPlayBudgetDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }

    public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

    public virtual DbSet<Currency> Currency { get; set; }

    public virtual DbSet<Expense> Expense { get; set; }

    public virtual DbSet<Income> Income { get; set; }

    public virtual DbSet<MonthlyBudgetInfo> MonthlyBudgetInfo { get; set; }

    public virtual DbSet<VwBalance> VwBalance { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRoles>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRoles",
                    r => r.HasOne<AspNetRoles>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUsers>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasOne(d => d.Currency).WithMany(p => p.Expense)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Expense_Currency");

            entity.HasOne(d => d.MonthlyBudgetInfo).WithMany(p => p.Expense)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Expense_MonthlyBudgetInfo");

            entity.HasOne(d => d.Owner).WithMany(p => p.Expense)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Expense_AspNetUsers");
        });

        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasOne(d => d.Currency).WithMany(p => p.Income)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Income_Currency");

            entity.HasOne(d => d.MonthlyBudgetInfo).WithMany(p => p.Income)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Income_MonthlyBudgetInfo");

            entity.HasOne(d => d.Owner).WithMany(p => p.Income)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Income_AspNetUsers");
        });

        modelBuilder.Entity<VwBalance>(entity =>
        {
            entity.ToView("vwBalance");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}