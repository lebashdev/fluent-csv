using System;
using System.Collections.Generic;

namespace LeBash.FluentCsv
{
    public interface ICsvTemplateBuilder<T, TRecord>
    {
        ICsvTemplate<T> Build();
        ICsvTemplateBuilder<T, TRecord> AddColumns(Action<List<(string, ColumnMapper<TRecord>)>> columnBuilder);
        ICsvTemplateBuilder<T, TRecord> WithProjector(Func<T, IEnumerable<TRecord>> projector);
        ICsvTemplateBuilder<T, TRecord> WithDelimiter(string delimiter);
    }
}