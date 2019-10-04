using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IdGen;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var idGen = new IdGenerator(404);
            foreach (var item in Enumerable.Range(1, 100))
            {
                Console.WriteLine($"New id {idGen.CreateId()}");
            }
            Console.ReadLine(); return;
            /*
            Expression<Func<int, bool>> exp = (i) => i > 0;

            var inv = Expression.Invoke(exp, Expression.Constant(4));
            Console.WriteLine(inv);

            var lamda = Expression.Lambda(inv.Expression, ParameterExpression.Parameter(typeof(int), "i"));
            var comp = lamda.Compile();
            var result = comp.DynamicInvoke(32);

            var table = "Expense";
            FormattableString ss = $"select * from {table}";
            var s = ss.ToString(new CustomFormatProvider());

            var a = new AnObject() { Description = "Giorgio" };
            Console.WriteLine("Hello World!");
            */
        }
    }

    public class CustomFormatProvider : IFormatProvider
    {
        private readonly ICustomFormatter customFormatter = new CustomFormatter();

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return customFormatter;
            return null;
        }

        public class CustomFormatter : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                return $"Null: {arg}";
            }
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
