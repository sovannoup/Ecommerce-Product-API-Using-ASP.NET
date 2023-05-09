using LicenseKey.Models;
using Microsoft.EntityFrameworkCore;

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
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<ContactMessage> ContactMessage { get; set; }
    }
}