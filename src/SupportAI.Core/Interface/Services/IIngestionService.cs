namespace SupportAI.Core.Interface.Services
{
    public interface IIngestionService
    {
        Task ProcessJsonAsync(string filePath);
        Task ProcessMarkdownAsync(string filePath);
        Task ProcessPdfAsync(string filePath);
    }
}
