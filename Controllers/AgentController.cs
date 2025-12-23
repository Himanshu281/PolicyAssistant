using Microsoft.AspNetCore.Mvc;
using PolicyAssistant.DTOs;
using PolicyAssistant.Services.Embeddings;
using PolicyAssistant.Services.LLM;
using PolicyAssistant.Services.Similarity;
using PolicyAssitant.Data;
using Microsoft.EntityFrameworkCore;

namespace PolicyAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly PolicyDbContext _db;
        private readonly IEmbeddingService _embeddingService;
        private readonly CosineSimilarityService _similarityService;
        private readonly ILLMService _llmService;

        public AgentController(
            PolicyDbContext db,
            IEmbeddingService embeddingService,
            CosineSimilarityService similarityService,
            ILLMService llmService)
        {
            _db = db;
            _embeddingService = embeddingService;
            _similarityService = similarityService;
            _llmService = llmService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            // 2. Convert question to embedding
            var questionEmbedding = await _embeddingService.GenerateEmbedding(request.Question);

            // 3. Load all policy chunks
            var chunks = await _db.PolicyChunks.AsNoTracking().ToListAsync();

            // 4. Find most relevant chunks
            var scoredChunks = chunks
                .Select(c => new
                {
                    Chunk = c,
                    Score = _similarityService.CosineSimilarity(
                        questionEmbedding,
                        DeserializeEmbedding(c.EmbeddingVector))
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            // 5. Guardrail: nothing relevant
            if (!scoredChunks.Any() || scoredChunks[0].Score < 0.7)
            {
                return Ok(new
                {
                    answer = "I don’t know. This is not covered in policy."
                });
            }

            // 6. Pick top chunks
            var context = string.Join(
                "\n",
                scoredChunks.Take(2).Select(x => x.Chunk.ChunkText)
            );

            // 7. Ask LLM to explain (later)
            var answer = await _llmService.GenerateAnswer(context, request.Question);

            // 8. Return answer
            return Ok(new
            {
                answer = answer
            });
        }

        private float[] DeserializeEmbedding(byte[] bytes)
        {
            var floats = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
            return floats;
        }
    }
}
