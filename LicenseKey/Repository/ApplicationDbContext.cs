using LicenseKey.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Metadata;

namespace LicenseKey.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ConfirmToken> ConfirmToken { get; set; }
        public virtual DbSet<UserTransaction> UserTransaction { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.LicenseKeyTo)
                    .HasConversion(
                        from => string.Join(":", from),
                        to => string.IsNullOrEmpty(to) ? new List<string>() : to.Split(':', StringSplitOptions.RemoveEmptyEntries).ToList(),
                        new ValueComparer<List<string>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()
                    )
                );
            });
        }
    }
}