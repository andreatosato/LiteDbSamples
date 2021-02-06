using Bogus;
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

            var testLog = new Faker<LogData>()
                .RuleFor(l => l.Name, t => t.Name.FirstName())
                .RuleFor(l => l.Surname, t => t.Name.LastName())
                .RuleFor(l => l.JobTitle, t => t.Name.JobTitle());

            foreach (var item in testLog.Generate(1000))
            {
                log.Information("Information: {@fakeObj}", item);
            }
            log.Dispose();
        }

        public class LogData
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public string Name { get; set; }
            public string Surname { get; set; }
            public string JobTitle { get; set; }
        }
    }
}
