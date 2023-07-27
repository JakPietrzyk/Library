using Microsoft.EntityFrameworkCore;

namespace Rental.Model
{
    public class RentalContext: DbContext
    {
        public DbSet<Customer> Customer {get;set;}
        public DbSet<Rent> Rent {get;set;}
        private string _connectionString =
            "DataSource=Database\\Rental.db";
        public RentalContext(){}
        public RentalContext(DbContextOptions<RentalContext> options) : base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                .IsRequired();
            modelBuilder.Entity<Customer>()
                .Property(c => c.Surname)
                .IsRequired();
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Rents)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Rent>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
            // modelBuilder.Entity<Book>()
            //     .HasOne(b => b.Customer)
            //     .WithMany(b => b.Books)
            //     .HasForeignKey(b => b.CustomerId)
            //     .IsRequired()
            //     .OnDelete(DeleteBehavior.Cascade);
                
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite(_connectionString);
        }
    }
}