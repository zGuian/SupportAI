using Microsoft.Extensions.AI;
using SupportAI.Core.Interface.Repository;
using SupportAI.Core.Interface.Services;
using System.Text.Json;
using UglyToad.PdfPig;

namespace SupportAI.Core.Services
{
    public class IngestionService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator
            , IQdrantRepository qdrantRepository) : IIngestionService
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
        private readonly IQdrantRepository _qdrantRepository = qdrantRepository;

        public async Task ProcessPdfAsync(string filePath)
        {
            using var pdf = PdfDocument.Open(filePath);
            foreach (var page in pdf.GetPages())
            {
                var text = page.Text;
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }
                await EmbedAndStoreAsync(text, new Dictionary<string, object>
                {
                    { "source", Path.GetFileName(filePath) },
                    { "page", page.Number },
                    { "text", "pdf" }
                });
            }
        }

        public Task ProcessMarkdownAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task ProcessJsonAsync(string filePath)
        {
            var jsonString = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString);

            if (data == null) return;

            foreach (var item in data)
            {
                string content = string.Join(" ", item.Values);

                await EmbedAndStoreAsync(content, new Dictionary<string, object>
                {
                    { "source", Path.GetFileName(filePath) },
                    { "type", "json" }
                });
            }
        }

        private async Task EmbedAndStoreAsync(string text, Dictionary<string, object> metadata)
        {
            var embedding = await _embeddingGenerator.GenerateAsync(text);
            await _qdrantRepository.SaveAsync(text, metadata, embedding);
        }
    }
}
