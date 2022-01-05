using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Kind> Kinds { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductStatus> ProductStatus { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<ProductsInBill> ProductsInBills { get; set; }
    }
}
