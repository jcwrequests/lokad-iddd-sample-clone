using System;
using System.Collections.Generic;
using System.IO;
using Sample.Domain;
using Sample.Storage;
using Sample.Storage.Files;
using Sample.Storage.MsSql;

namespace Sample
{
    public static class Program
    {
        public static void Main()
        {
            if (File.Exists("Readme.md"))
                Console.WriteLine(File.ReadAllText("Readme.md"));

            // persistence
            var store = CreateFileStoreForTesting();
            var events = new EventStore(store);

            // various domain services
            var pricing = new PricingService();

            var server = new ApplicationServer();
            server.Handlers.Add(new LoggingWrapper(new CustomerApplicationService(events, pricing)));

            // send some sample commands
            server.Dispatch(new CreateCustomer {Id = new CustomerId(12), Name = "Lokad", Currency = Currency.Eur});
            server.Dispatch(new RenameCustomer {Id = new CustomerId(12), NewName = "Lokad SAS"});
            server.Dispatch(new ChargeCustomer {Id = new CustomerId(12), Amount = 20m.Eur(), Name = "Forecasting"});

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        static IAppendOnlyStore CreateSqlStore()
        {
            var conn = "Data Source=.\\SQLExpress;Initial Catalog=lokadsalescast_samples;Integrated Security=true";
            var store = new SqlAppendOnlyStore(conn);
            store.Initialize();
            return store;
        }

        static IAppendOnlyStore CreateFileStoreForTesting()
        {
            var combine = Path.Combine(Directory.GetCurrentDirectory(), "store");
            if (Directory.Exists(combine))
            {
                Console.WriteLine();
                Console.WriteLine("Wiping file event store for demo purposes.");
                Console.WriteLine("You can switch to Azure or SQL event stores by modifying Program.cs");
                Console.WriteLine();
                Directory.Delete(combine, true);
            }
            var store = new FileAppendOnlyStore(combine);
            store.Initialize();
            return store;
        }

        /// <summary>
        /// This is a simplified representation of real application server. 
        /// In production it is wired to messaging and/or services infrastructure.</summary>
        public sealed class ApplicationServer
        {
            public void Dispatch(ICommand cmd)
            {
                foreach (var handler in Handlers)
                {
                    handler.Execute(cmd);
                }
            }

            public readonly IList<IApplicationService> Handlers = new List<IApplicationService>();
        }
    }
}