using System.Threading.Tasks;

namespace LeBash.FluentCsv
{
    public interface ICsvTemplate<in T>
    {
        Task RenderToFileAsync(string filePath, T data);
    }
}