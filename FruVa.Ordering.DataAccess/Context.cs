using FruVa.Ordering.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace FruVa.Ordering.DataAccess
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Orders;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True");
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
