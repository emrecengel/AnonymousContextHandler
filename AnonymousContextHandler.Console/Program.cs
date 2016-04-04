using System;
using System.Configuration;
using System.Linq;

namespace AnonymousContextHandler.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            // var guid = "a80c85b0-0df2-48a4-b400-8ae82405b9cd";
            var guid = Guid.NewGuid().ToString();
            var modelRepository = AnonymousContext.AzureTableRepository<Transaction>(x => x.Id, ConfigurationSettings.AppSettings.Get("StorageConnectionString"), guid);


/*

            for (int i = 0; i < 15; i++)
            {
                if (i != 10)
                    modelRepository.Add(new Transaction(i.ToString(), 10 * i, 10, DateTime.Now));

            }
*/
            var tt = modelRepository.List(query => query.Where(x => true));
        }
    }
}
