using Assignment_CS5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Assignment_CS5.Database
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Customer> Customer { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          modelBuilder.Entity<Menu>().ToTable("Menu");
          modelBuilder.Entity<Employee>().ToTable("Employee");
          modelBuilder.Entity<Order>().ToTable("Order");
          modelBuilder.Entity<OrderDetails>().ToTable("OrderDetails");
          modelBuilder.Entity<Customer>().ToTable("Customer");
        }
    }
}
