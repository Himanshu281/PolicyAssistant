namespace PolicyAssistant.Services.LLM
{
    public class OllamaLLMService : ILLMService
    {
        private readonly HttpClient _httpClient;

        public OllamaLLMService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateAnswer(string policyContext, string question)
        {
            var prompt = $@"You are a policy assistant. Answer ONLY using the policy text below. Policy:{policyContext} Question: {question}";

            var request = new
            {
                model = "llama3",
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request
            );

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

            return result.response;
        }
    }

    public class OllamaGenerateResponse
    {
        public string response { get; set; }
    }
}
