using System;

namespace LRUCache
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var cache = new LRUCache<int>(3);

            cache.Add("3", 3); 
            Console.WriteLine(cache.DebugInfo());
            cache.Add("6", 6);
            cache.Add("13", 3);
            cache.Add("16", 6); 
            Console.WriteLine(cache.DebugInfo());

            cache.Get("6");

            Console.WriteLine(cache.DebugInfo());
            Console.WriteLine("Hello World!");
        }
    }
}
