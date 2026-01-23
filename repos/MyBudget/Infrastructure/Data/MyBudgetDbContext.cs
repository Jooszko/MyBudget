using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBudget.Domain.Models;
using MyBudget.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Infrastructure.Data
{
    public class MyBudgetDbContext : IdentityDbContext<AppUser>
    {
        public MyBudgetDbContext(DbContextOptions<MyBudgetDbContext> op) : base(op)
        {

        }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Income> Incomes => Set<Income>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Owned<Money>();

            builder.Entity<Expense>(b =>
            {
                b.OwnsOne(e => e.Money, m =>
                {
                    m.Property(p => p.Amount).HasColumnName("Amount");
                    m.Property(p => p.CurrencyCode).HasColumnName("CurrencyCode").HasMaxLength(3);
                });

                b.HasIndex(x => x.UserId);

                b.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Income>(b =>
            {
                b.OwnsOne(i => i.Money, m =>
                {
                    m.Property(p => p.Amount).HasColumnName("Amount");
                    m.Property(p => p.CurrencyCode).HasColumnName("CurrencyCode").HasMaxLength(3);
                });

                b.HasIndex(x => x.UserId);
            });

            builder.Entity<Category>()
                .HasIndex(x => new { x.UserId, x.Name })
                .IsUnique();
        }

    }
}
