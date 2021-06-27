using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LeBash.FluentCsv.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Source object that we want to render as CSV.
            var report = GetData();

            // Create a template definition.
            var template = CsvTemplateBuilder.Create<MyReport, Record>()
                .AddColumns(columns =>
                {
                    columns.Add(("Id", x => x.Id.ToString()));
                    columns.Add(("Name", x => x.Name));
                })
                .WithProjector(data => data.Items)
                .WithDelimiter(";")
                .Build();

            var directory = "c:\\temp";

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            // Render the CSV to a file.
            await template.RenderToFileAsync(Path.Combine(directory, "hello.csv"), report);
        }

        static MyReport GetData()
        {
            var report = new MyReport
            {
                Items = new List<Record>
                {
                    new Record {Id = 1, Name = "Bob"},
                    new Record {Id = 2, Name = "Alice"},
                    new Record {Id = 3, Name = "Jason"},
                    new Record {Id = 4, Name = "Kurt"},
                    new Record {Id = 5, Name = "Bruce \"The Boss\" Springsteen"}
                }
            };

            return report;
        }
    }

    class Record
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class MyReport
    {
        public List<Record> Items { get; set; }
    }
}
