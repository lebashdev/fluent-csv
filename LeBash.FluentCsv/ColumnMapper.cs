namespace LeBash.FluentCsv
{
    public delegate string ColumnMapper<in TSource>(TSource source);
}