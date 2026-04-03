using Microsoft.Extensions.AI;

namespace SupportAI.Core.Interface.Repository
{
    public interface IQdrantRepository
    {
        Task SaveAsync(string text, Dictionary<string, object> metadata, Embedding<float> embedding);
    }
}
