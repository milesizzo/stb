using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScratchSpace
{
    class Program
    {
        public static Boolean IsAnonymousType(Type type)
        {
            Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            Boolean isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        private static void PrintObject(object o)
        {
            Console.WriteLine(o.GetType() == typeof(int) ? "int" : "not int");
            Console.WriteLine(o.GetType() == typeof(float) ? "float" : "not float");
            Console.WriteLine(o.GetType() == typeof(string) ? "string" : "not string");
            Console.WriteLine(IsAnonymousType(o.GetType()) ? "anon" : "not anon");
        }

        static void Main(string[] args)
        {
            var x = new
            {
                Blah = "hello",
            };

            foreach (var prop in x.GetType().GetProperties())
            {
                Console.WriteLine(prop.Name);
                Console.WriteLine(prop.GetValue(x));
            }

            PrintObject(5);
            PrintObject(5.3f);
            PrintObject("hello");
            PrintObject(x);

            Console.ReadLine();
        }
    }
}
