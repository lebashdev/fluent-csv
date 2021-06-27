using System;
using System.Collections.Generic;

namespace LeBash.FluentCsv
{
    /// <summary>
    /// Represents a CSV template builder.
    /// It is used to configure the behavior of the CSV generator.
    /// </summary>
    /// <typeparam name="TData">The type of the data being converted to CSV.</typeparam>
    /// <typeparam name="TRecord">The type of data being mapped to CSV rows.</typeparam>
    public interface ICsvTemplateBuilder<TData, TRecord>
    {
        /// <summary>
        /// Build the template after it's been configured.
        /// </summary>
        /// <returns>A CSV template.</returns>
        ICsvTemplate<TData> Build();

        /// <summary>
        /// Add column definitions to the template.
        /// </summary>
        /// <param name="columnBuilder">A column builder, including the name and a function to map a raw value to its text representation.</param>
        /// <returns>The builder.</returns>
        ICsvTemplateBuilder<TData, TRecord> AddColumns(Action<List<(string, ColumnMapper<TRecord>)>> columnBuilder);

        /// <summary>
        /// Specifies how the logical document should map to a collection of logical rows.
        /// </summary>
        /// <param name="projector">A function that takes the raw logic document and converts it to a sequence of rows.</param>
        /// <returns>The builder.</returns>
        ICsvTemplateBuilder<TData, TRecord> WithProjector(Func<TData, IEnumerable<TRecord>> projector);

        /// <summary>
        /// Specify what character sequence should be used to delimit CSV columns.
        /// </summary>
        /// <param name="delimiter">A string.</param>
        /// <returns>The builder.</returns>
        ICsvTemplateBuilder<TData, TRecord> WithDelimiter(string delimiter);
    }
}