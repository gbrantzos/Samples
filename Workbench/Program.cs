using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var a = new AnObject() { Description = "Giorgio" };
            Console.WriteLine("Hello World!");

        }
    }


    public class AnObject
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Type ProgramType { get; set; }

        public void AddProperty(Expression<Func<AnObject, object>> expression)
        {
            // Also look on https://stackoverflow.com/a/672212
            var r1 = expression.Compile().Invoke(this);
            var memberExpression = expression.Body as MemberExpression;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            var r = propertyInfo.GetValue(this, null).ToString();
        }

        public AnObject()
        {
            Description = "Giorgio";
            this.AddProperty(a => a.Description);
        }
    }
}
