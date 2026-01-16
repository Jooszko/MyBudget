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
        public MyBudgetDbContext(DbContextOptions<MyBudgetDbContext> op): base(op)
        {
            
        }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Income> Incomes => Set<Income>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>()
                .HasIndex(x => new { x.UserId, x.Name })
                .IsUnique();

            builder.Entity<Expense>()
              .HasOne<Category>()
              .WithMany()
              .HasForeignKey(x => x.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Expense>().HasIndex(x => x.UserId);
            builder.Entity<Income>().HasIndex(x => x.UserId);
        }
    }
}
