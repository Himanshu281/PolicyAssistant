namespace PolicyAssistant.Services.Embeddings
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbedding(string text);
    }
}
