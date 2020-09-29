using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Sinks.Graylog;
using Workbench;

namespace Sandbox
{
    public static class Program
    {
        public abstract class Base { }

        public class Child : Base { }

        public static void Process(Base anObj)
        {
            Console.WriteLine(anObj.GetType().Name);
        }

        public static async Task Main_OLD(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddMyConfiguration();

            var configuration = configurationBuilder.Build();
            ChangeToken.OnChange(
                () => configuration.GetReloadToken(),
                () => {
                    Console.WriteLine("Configuration changed!");
                });

            var v1 = configuration.GetSection("Test1").Value;
            Console.WriteLine($"Value {v1}");
            

            await Task.CompletedTask;
            Console.ReadLine();
            
            var v2 = configuration.GetSection("Test1").Value;
            Console.WriteLine($"Value {v2}");
            return;
            /*
            var ta = new TasksWhenAny();
            Console.WriteLine($"Starting at {DateTime.Now}");

            var cts = new CancellationTokenSource();
            var q1 = ta.ExecuteQuery("0:00:40", cts.Token);
            var q2 = ta.ExecuteQuery("0:00:10", cts.Token);

            var result = await Task.WhenAny(q1, q2);
            Console.WriteLine($"Finished at {DateTime.Now}");

            var actualResult = await result;
            Console.WriteLine($"Result is: {actualResult}");
            cts.Cancel();

            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console()
            //    .WriteTo.Graylog(new GraylogSinkOptions
            //    {
            //        Facility = "test",
            //        HostnameOrAddress = "graylog.gbworks.lan",
            //        Port = 12201,
            //        TransportType = Serilog.Sinks.Graylog.Core.Transport.TransportType.Udp
            //    })
            //    .CreateLogger();

            //Log.Information("Hello from Console");
            //Log.Error("This is an error!");
            //Log.Warning("Here comes a {severe} warning!", "Duffy");
            
            var child = new Child();
            Process(child);

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
            */
            Console.ReadLine();
        }

        public static void Main(string[] args)
        {
            ISessionManager mgr = new SessionManager(new SessionPool());
            using (var session = mgr.Create())
            {
                using (var session2 = mgr.Create())
                {
                }

            }
            using var session1 = mgr.Create();

            Console.ReadKey();
        }

        private static void OnOutbound(double result)
        {
            Console.WriteLine($"Result: {result}");
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
