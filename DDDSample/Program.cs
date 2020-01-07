using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DDDSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = "Server=(local);Database=DDDSample;Trusted_Connection=True;MultipleActiveResultSets=true";
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connectionString);

            using (var db = new DataContext(optionsBuilder.Options))
            {
                db.Database.EnsureCreated();

                //var order = new Order(123);
                //order.AddItem(new OrderItem { Product = "Product 1", Quantity = 3 });
                //db.Add(order);

                var order = db.Orders.Include(o => o.Items).SingleOrDefault(o => o.Id == 1);
                order.AddItem(new OrderItem("Product 1", 31));
                order.RemoveItem("Product 4");

                db.SaveChanges();

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    public class Order
    {
        public int Id { get; private set; }
        public double Number { get; private set; }

        public Order(double number) => Number = number;

        private List<OrderItem> items = new List<OrderItem>();
        public IEnumerable<OrderItem> Items => items.AsReadOnly();

        public void AddItem(OrderItem orderItem) => items.Add(orderItem);
        public void RemoveItem(string product)
        {
            var orderItem = this.items.First(i => i.Product == product);
            items.Remove(orderItem);
        }
    }

    public class OrderItem
    {
        public int Id { get; private set; }
        public string Product { get; private set; }
        public double Quantity { get; private set; }

        public OrderItem(string product, double quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<Order>()
                .Metadata
                .FindNavigation(nameof(Order.Items))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder
                .Entity<Order>()
                .HasMany(nameof(Order.Items))
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
