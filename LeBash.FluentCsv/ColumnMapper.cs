namespace LeBash.FluentCsv
{
    /// <summary>
    /// Represents a function that maps a value to a string.
    /// </summary>
    /// <typeparam name="TSource">The type of value being converted.</typeparam>
    /// <param name="source">The value being converted.</param>
    /// <returns>A string.</returns>
    public delegate string ColumnMapper<in TSource>(TSource source);
}