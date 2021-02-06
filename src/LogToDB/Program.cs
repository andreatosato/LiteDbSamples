using Serilog;
using System;

namespace LogToDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var log = new LoggerConfiguration()
                .WriteTo.LiteDB(@$"{AppContext.BaseDirectory}\logs\applog.db", logCollectionName: "applog")
                .CreateLogger();

            log.Error("Errore");
        }
    }
}
