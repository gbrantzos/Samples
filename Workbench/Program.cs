using System;
using Workbench;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
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
}
