using System;
using System.Linq;
using Test.Console;

namespace AnonymousContextHandler.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var modelRepository = AnonymousContext.IoModelRepository<Transaction>(x => x.Id, "C:/temp/ComwebTest/", Guid.NewGuid()
                     .ToString());
            for (int i = 0; i < 10; i++)
            {
                modelRepository.Add(new Transaction(i.ToString(), 10 * i, 10, DateTime.Now));
                System.Console.Clear();
                System.Console.WriteLine(string.Join(Environment.NewLine,modelRepository.List(query => query.Where(x => true)).Select(x => string.Format("{0} | {1}", x.Id, x.Description))));
            }

        }
    }
}
