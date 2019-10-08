using System;

namespace Workbench
{
    public class Service : IService
    {
        public void Do(string message)
        {
            Console.WriteLine(message);
        }
    }
}
