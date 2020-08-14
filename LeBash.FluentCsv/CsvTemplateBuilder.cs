using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeBash.FluentCsv
{
    public static class CsvTemplateBuilder
    {
        public static ICsvTemplateBuilder<T, TRecord> Create<T, TRecord>()
        {
            return new CsvTemplateBuilder<T, TRecord>();
        }

        public static ICsvTemplateBuilder<IEnumerable<TRecord>, TRecord> Create<TRecord>()
        {
            return new CsvTemplateBuilder<TRecord>();
        }
    }

    internal class CsvTemplateBuilder<TRecord> : CsvTemplateBuilder<IEnumerable<TRecord>, TRecord>
    {
    }

    internal class CsvTemplateBuilder<T, TRecord> : ICsvTemplateBuilder<T, TRecord>
    {
        private readonly CsvTemplate _template = new CsvTemplate();
        private readonly List<(string, ColumnMapper<TRecord>)> _columns = new List<(string, ColumnMapper<TRecord>)>();

        public ICsvTemplate<T> Build()
        {
            this._template.ColumnHeaders = this._columns.Select(x => x.Item1).ToList();
            this._template.ColumnsAccessors = this._columns.Select(x => x.Item2).ToList();

            if (this._template.ColumnHeaders.Count == 0)
            {
                throw new Exception("No columns have been defined.");
            }

            if (this._template.Iterator == null)
            {
                // Can we infer a default enumerator from the data?
                if (typeof(T).IsAssignableFrom(typeof(IEnumerable<TRecord>)))
                {
                    this._template.Iterator = source => source as IEnumerable<TRecord>;
                }
                else
                {
                    throw new Exception("A projector must be specified.");
                }
            }

            return this._template;
        }

        public ICsvTemplateBuilder<T, TRecord> AddColumns(Func<IEnumerable<(string, ColumnMapper<TRecord>)>> columnBuilder)
        {
            this._columns.AddRange(columnBuilder());

            return this;
        }

        public ICsvTemplateBuilder<T, TRecord> WithProjector(Func<T, IEnumerable<TRecord>> projector)
        {
            this._template.Iterator = projector;

            return this;
        }

        public ICsvTemplateBuilder<T, TRecord> WithDelimiter(string delimiter)
        {
            this._template.Delimiter = delimiter;

            return this;
        }

        private sealed class CsvTemplate : ICsvTemplate<T>
        {
            public List<string> ColumnHeaders { get; set; }
            public List<ColumnMapper<TRecord>> ColumnsAccessors { get; set; }
            public Func<T, IEnumerable<TRecord>> Iterator { get; set; }
            public string Delimiter { get; set; } = ",";

            public async Task RenderToFileAsync(string filePath, T data)
            {
                await using var file = File.CreateText(filePath);
                var records = this.Iterator(data);

                await file.WriteLineAsync($"sep={this.Delimiter}");
                await file.WriteLineAsync(String.Join(this.Delimiter, this.ColumnHeaders.Select(GetCellValue)));

                foreach (var record in records)
                {
                    await file.WriteLineAsync(String.Join(
                        this.Delimiter,
                        this.ColumnsAccessors.Select(f => GetCellValue(f(record)))));
                }
            }

            private static string GetCellValue(string value)
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
        }
    }
}