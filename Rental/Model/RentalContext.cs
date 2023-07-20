using Microsoft.EntityFrameworkCore;

namespace Rental.Model
{
    public class RentalContext: DbContext
    {
        public DbSet<Customer> Customer {get;set;}
        private string _connectionString =
            "DataSource=Database\\Rental.db";
        public RentalContext(){}
        public RentalContext(DbContextOptions<RentalContext> options) : base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                .IsRequired();
            modelBuilder.Entity<Customer>()
                .Property(c => c.Surname)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}