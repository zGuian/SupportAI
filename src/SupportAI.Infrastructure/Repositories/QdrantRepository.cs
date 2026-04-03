using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using SupportAI.Core.Interface.Repository;

namespace SupportAI.Infrastructure.Repositories
{
    public class QdrantRepository(QdrantClient qdrantClient) : IQdrantRepository
    {
        private readonly QdrantClient _qdrantClient = qdrantClient;
        private const string CollectionName = "forum_conhecimento";

        public async Task SaveAsync(string text, Dictionary<string, object> metadata, Embedding<float> embedding)
        {
            var point = new PointStruct
            {
                Id = Guid.NewGuid(),
                Vectors = embedding.Vector.ToArray(),
                Payload = { { "content", text } }
            };

            foreach (var item in metadata)
            {
                point.Payload.Add(item.Key, item.Value.ToString());
            }

            await _qdrantClient.UpsertAsync(CollectionName, [point]);
        }
    }
}
