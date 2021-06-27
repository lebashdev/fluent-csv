using System.Threading.Tasks;

namespace LeBash.FluentCsv
{
    /// <summary>
    /// Represents a CSV template.
    /// </summary>
    /// <typeparam name="T">The of data being rendered to CSV.</typeparam>
    public interface ICsvTemplate<in T>
    {
        /// <summary>
        /// Convert the specified data to CSV and write it to the specified path.
        /// </summary>
        /// <param name="filePath">The name of the file that will be generated.</param>
        /// <param name="data">The data to convert to CSV.</param>
        /// <returns>A Task.</returns>
        Task RenderToFileAsync(string filePath, T data);
    }
}