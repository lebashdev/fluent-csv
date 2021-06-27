using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeBash.FluentCsv
{
    /// <summary>
    /// A template builder uses the builder pattern to
    /// generate a CSV file definition.
    /// </summary>
    public static class CsvTemplateBuilder
    {
        /// <summary>
        /// Create a template builder.
        /// </summary>
        /// <typeparam name="TData">The type of data being converted to CSV.</typeparam>
        /// <typeparam name="TRecord">The type of value mapped to CSV rows.</typeparam>
        /// <returns>A CSV template builder.</returns>
        public static ICsvTemplateBuilder<TData, TRecord> Create<TData, TRecord>()
        {
            return new CsvTemplateBuilder<TData, TRecord>();
        }

        /// <summary>
        /// Create a template builder.
        /// </summary>
        /// <typeparam name="TRecord">The type value value mapped to CSV rows.</typeparam>
        /// <returns>A CSV template builder.</returns>
        public static ICsvTemplateBuilder<IEnumerable<TRecord>, TRecord> Create<TRecord>()
        {
            return new CsvTemplateBuilder<TRecord>();
        }
    }

    /// <summary>
    /// A CSV template builder where the input document is a collection of
    /// records directly mappable to the output CSV.
    /// </summary>
    /// <typeparam name="TRecord">The type of value mapped to CSV rows.</typeparam>
    internal class CsvTemplateBuilder<TRecord> : CsvTemplateBuilder<IEnumerable<TRecord>, TRecord>
    {
    }

    /// <summary>
    /// A CSV template builder.
    /// </summary>
    /// <typeparam name="TData">The type of the logical document being mapped to CSV.</typeparam>
    /// <typeparam name="TRecord">The type of value mapped to CSV rows.</typeparam>
    internal class CsvTemplateBuilder<TData, TRecord> : ICsvTemplateBuilder<TData, TRecord>
    {
        private readonly CsvTemplate _template = new CsvTemplate();
        private readonly List<(string, ColumnMapper<TRecord>)> _columns = new List<(string, ColumnMapper<TRecord>)>();

        /// <summary>
        /// Build the template after it's been configured.
        /// </summary>
        /// <returns>A CSV template.</returns>
        public ICsvTemplate<TData> Build()
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
                if (typeof(TData).IsAssignableFrom(typeof(IEnumerable<TRecord>)))
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

        /// <summary>
        /// Add column definitions to the template.
        /// </summary>
        /// <param name="columnBuilder">A column builder, including the name and a function to map a raw value to its text representation.</param>
        /// <returns>The builder.</returns>
        public ICsvTemplateBuilder<TData, TRecord> AddColumns(Action<List<(string, ColumnMapper<TRecord>)>> columnBuilder)
        {
            columnBuilder(this._columns);

            return this;
        }

        /// <summary>
        /// Specifies how the logical document should map to a collection of logical rows.
        /// </summary>
        /// <param name="projector">A function that takes the raw logic document and converts it to a sequence of rows.</param>
        /// <returns>The builder.</returns>
        public ICsvTemplateBuilder<TData, TRecord> WithProjector(Func<TData, IEnumerable<TRecord>> projector)
        {
            this._template.Iterator = projector;

            return this;
        }

        /// <summary>
        /// Specify what character sequence should be used to delimit CSV columns.
        /// </summary>
        /// <param name="delimiter">A string.</param>
        /// <returns>The builder.</returns>
        public ICsvTemplateBuilder<TData, TRecord> WithDelimiter(string delimiter)
        {
            this._template.Delimiter = delimiter;

            return this;
        }

        /// <summary>
        /// Implementation of ICsvTemplate<TData>.
        /// </summary>
        private sealed class CsvTemplate : ICsvTemplate<TData>
        {
            public List<string> ColumnHeaders { get; set; }
            public List<ColumnMapper<TRecord>> ColumnsAccessors { get; set; }
            public Func<TData, IEnumerable<TRecord>> Iterator { get; set; }
            public string Delimiter { get; set; } = ",";

            public async Task RenderToFileAsync(string filePath, TData data)
            {
                // Create the output file.
                await using var file = File.CreateText(filePath);
                var records = this.Iterator(data);

                // Write the CSV header.
                await file.WriteLineAsync($"sep={this.Delimiter}");

                // Write the column names (first row).
                await file.WriteLineAsync(String.Join(this.Delimiter, this.ColumnHeaders.Select(GetCellValue)));

                // Render a row for each record.
                foreach (var record in records)
                {
                    await file.WriteLineAsync(String.Join(
                        this.Delimiter,
                        this.ColumnsAccessors.Select(f => GetCellValue(f(record)))));
                }
            }

            private static string GetCellValue(string value)
            {
                // Wrap the cell values in quotes and escape internal quotes.
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
        }
    }
}