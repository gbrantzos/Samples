using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Workbench;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var agreement = new Agreement
            {
                ID = 101,
                Description = "Convert to dictionary",
                DateOfBirth = new DateTime(1976, 6, 3),
                Customer = new Customer
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Age = 30,
                    Address = new List<Address>()
                    {
                        new Address
                        {
                            Name = "Los Angeles 1",
                            ZipCode = 25437
                        },
                        new Address
                        {
                            Name = "New York 25",
                            ZipCode = 25441
                        }
                    }
                }
            };

            var result = agreement.ToConfigData(nameof(Agreement));
            Console.WriteLine("Agreement");
            Console.WriteLine(new String('=', 80));
            Console.WriteLine();

            foreach (var item in result)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(result)
                .Build();
            var fromConfig = config.GetSection(nameof(Agreement)).Get<Agreement>();
            Console.WriteLine();
            Console.WriteLine($"Random key: {KeyGenerator.GetUniqueKey(32)}");

            ConsoleUtility.WriteProgressBar(0);
            for (var i = 0; i <= 100; ++i)
            {
                ConsoleUtility.WriteProgressBar(i, true);
                System.Threading.Thread.Sleep(50);
            }
            Console.WriteLine();
            ConsoleUtility.WriteProgress(0);
            for (var i = 0; i <= 100; ++i)
            {
                ConsoleUtility.WriteProgress(i, true);
                System.Threading.Thread.Sleep(50);
            }

            Console.ReadLine();
        }
    }

    internal class Agreement
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public Customer Customer { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    internal class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public IEnumerable<Address> Address { get; set; }
    }

    internal class Address
    {
        public string Name { get; set; }
        public int ZipCode { get; set; }
    }
}
