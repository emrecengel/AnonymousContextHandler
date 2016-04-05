using System;
using System.Configuration;
using System.Linq;
using Test.Console;

namespace AnonymousContextHandler.Console
{
    class Program
    {
        static void Main(string[] args)
        {

          var repository =   AnonymousContext.IoModelRepository<AuditLog>(x => x.Id, "C:/Temp/", Guid.NewGuid()
                .ToString());

            repository.Add(new AuditLog() {CreatedOn = DateTime.Now, LogInfo = "Log", CustomerId = 303});

        }
    }
}
