using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Models
{
    public class ACMEDbContext: DbContext
    {
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<Stock> Stock{ get; set; }
        public DbSet<Order> Orders{ get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set;}
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<ProductShoppingCart> ProductShoppingCarts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-GA32F87\\SQLEXPRESS;Initial Catalog=ACME;Integrated Security=True");
            //optionsBuilder.UseSqlServer("Data Source=LAPTOP-KFVM6M83\\SQLEXPRESS;Initial Catalog=ACME;Integrated Security=True");
        }
    }
}
