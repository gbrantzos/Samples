using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace Sandbox
{
    public static class Program
    {
        public static void Main2(string[] args) => Decorators.Program.MainDec(args);

        public static void Main(string[] args)
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var e = Where(list, i => i % 2 == 0);
            foreach (var i in e.Where(_ => true))
            {
                Console.WriteLine(i);
            }
            Console.ReadLine();
        }

        private static IEnumerable<int> Where(IEnumerable<int> list, Predicate<int> pr)
        {
            foreach (var i in list)
            {
                Console.WriteLine(i);
                if (pr(i))
                    yield return i;
            }
        }

        public class C1
        {
            private readonly int _val;

            public C1(int val) => _val = val;

            public C1 Add(C1 toAdd) => new C1(_val + toAdd._val);
        }

        public static void Main1(string[] args)
        {
            var person = new Person
            {
                Fullname = "Giorgio",
                ContactInfo = new[]
                {
                    new ContactInfo { Email = "g.brantzos@gmail.com", EmailType = EmailType.Personal }
                }.ToList()
            };

            var personNew = new Person
            {
                Fullname = "Giorgos",
                ContactInfo = new[]
                {
                    new ContactInfo { Email = "g.brantzos@gmail.com", EmailType = EmailType.Work },
                    new ContactInfo { Email = "g.brantzos@outlook.com", EmailType = EmailType.Other },
                }.ToList()
            };

            var connectionString = "Server=(local);Database=Sample;Trusted_Connection=true;";
            var optionsBuilder = new DbContextOptionsBuilder<SampleContext>();
            optionsBuilder.UseSqlServer(connectionString);
            using (var db = new SampleContext(optionsBuilder.Options))
            {
                db.ChangeTracker.TrackGraph(personNew, e =>
                {
                    e.Entry.State = EntityState.Unchanged;
                    if (e.Entry.Entity is Person)
                    {
                        //int i = 1;
                    }
                    if (e.Entry.Entity is ContactInfo)
                    {
                        //int v = 2;
                    }
                });
            }

            /* var fp = new PhysicalFileProvider("C:\\Deploys");

           ChangeToken.OnChange(
               () => fp.Watch("my.zip"),
               state =>
               {
                   var st = new
                   {
                       FileInfo = fp.GetFileInfo("my.zip")
                   };
                   Console.WriteLine($"Name: {st.FileInfo.Name}, size: {st.FileInfo.Length}");
               },
               "");

           Console.ReadKey();

           var processor = new MyProcessor();
           var workItems = Enumerable
               .Range(1, 40)
               .Select(i => $"Payload of item number {i}")
               .ToList();
           processor.AddRange(workItems);*/

            Console.WriteLine("Hi!");
            Console.ReadLine();
        }
    }

    public class Person
    {
        public int ID { get; set; }
        public string Fullname { get; set; }
        public string Category { get; set; }
        public ICollection<ContactInfo> ContactInfo { get; set; } = new HashSet<ContactInfo>();
    }

    public class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions<SampleContext> options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
    }

    public class MyProcessor : WorkItemProcessor<string>
    {
        public MyProcessor() : base(5) { }

        protected override async Task Process(WorkItem<string> workItem, int worker)
        {
            await Task.Delay(40);
            Console.WriteLine($"Worker: {worker}, {workItem.Payload}, {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
