using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SqlKata;
using SqlKata.Compilers;
using Workbench;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var fileName = @"C:\wrk_Temp\InfoSupport\pakoworld.xml";
            var xml = XDocument.Load(fileName);


            // DataMedia
            var result = xml
                .Root
                .Elements("products")
                .Elements("product")
                .Take(10)
                .Select(x => new
                {
                    Sku = x.Element("sku").Value,
                    Available = x.Element("instock").Value == "Y"
                })
                .ToDictionary(e => e.Sku, e => e.Available);

            // Iliadis
            //var result = xml
            //    .Root
            //    .Elements("products")
            //    .Elements("product")
            //    .Take(10)
            //    .Select(x => new
            //    {
            //        sku = x.Element("sku").Value,
            //        available = x.Element("instock").Value == "Y"
            //    }).ToList();
        }

        public static void Main_SqlKata(string[] args)
        {
            // Create an instance of SQLServer
            var compiler = new SqlServerCompiler();

            var query = new Query("Users as u")
                .Join("Categories as c", "c.Id", "c.Category_ID")
                .Where("c.Id", 1)
                .Where("c.Status", "Active");

            SqlResult result = compiler.Compile(query);

            string sql = result.Sql;
            List<object> bindings = result.Bindings; // [ 1, "Active" ]
        }

        public static void Main_Result(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Warn("Testing");
            Result<int> result = 32;
            var r2 = Result.Ok(45);

            if (result.HasErrors)
            {
                var fail = Result.Fail("This is a failure");

                var output = result.When(() => "Success", () => "Failure!");
                var oo2 = result.When((i) => $"Result is {i}", (s) => s);
            };
        }

        public static void Main_Session(string[] args)
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
    }

    public interface IService<T> where T : DTO, new()
    {
        T Get(string key);
    }

    public interface IServiceExt : IService<DTO>
    {
        void Do();
    }

    public class Service<T> : IService<T> where T: DTO, new()
    {
        public T Get(string key)
        {
            return new T { Name = key };
        }
    }
    public class DTO
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class DTO2 : DTO { }

    public class ServiceDTO : IServiceExt
    {
        public void Do()
        {
            // throw new NotImplementedException();
        }

        public DTO Get(string key)
        {
            return new DTO { Name = $"Advanced {key}" };
        }
    }
}
