namespace PolicyAssistant.Services.LLM
{
    public interface ILLMService
    {
        Task<string> GenerateAnswer(string policyContext, string question);
    }
}
