using PolicyAssistant.DTOs;

namespace PolicyAssistant.Services.Embeddings
{
    public class OllamaEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;

        public OllamaEmbeddingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float[]> GenerateEmbedding(string text)
        {
            var request = new
            {
                model = "nomic-embed-text",
                prompt = text
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/embeddings",
                request
            );

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<OllamaEmbeddingResponse>();

            return result?.embedding ?? Array.Empty<float>();
        }
    }

}
