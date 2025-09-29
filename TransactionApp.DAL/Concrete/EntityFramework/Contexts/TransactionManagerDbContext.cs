using Microsoft.EntityFrameworkCore;
using TransactionApp.ENTITIES.Concrete.TransactionManager;

namespace TransactionApp.DAL.Concrete.EntityFramework.Contexts
{
    public class TransactionManagerDbContext : DbContext
    {
        public TransactionManagerDbContext(DbContextOptions<TransactionManagerDbContext> options)
        : base(options)
        { }

        public DbSet<USER> USERS { get; set; } = null!;
        public DbSet<TRANSACTION> TRANSACTIONS { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER entity configuration
            modelBuilder.Entity<USER>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                // One-to-many relationship with TRANSACTION
                entity.HasMany(e => e.Transactions)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TRANSACTION entity configuration
            modelBuilder.Entity<TRANSACTION>(entity =>
            {
                entity.HasKey(e => e.Id);
                //One-to-one relationship with USER
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.TransactionType).IsRequired().HasConversion<string>();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
