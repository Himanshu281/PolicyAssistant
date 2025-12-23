using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolicyAssistant.DTOs;
using PolicyAssistant.Services.Embeddings;
using PolicyAssitant.Data;
using PolicyAssitant.Models;

namespace PolicyAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly PolicyDbContext _db;
        private readonly IEmbeddingService _embeddingService;

        public PoliciesController(PolicyDbContext db, IEmbeddingService embeddingService)
        {
            _db = db;
            _embeddingService = embeddingService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPolicyAsync(IEnumerable<PolicyWriteModel> data)
        {
            var entities = data.Select(d => new PolicyDocument
            {
                Title = d.Title,
                ContentText = d.ContentText
            });

            await _db.PolicyDocuments.AddRangeAsync(entities);
            await _db.SaveChangesAsync();

            return Ok(data);
        }

        [HttpPost("reindex")]
        public async Task<IActionResult> ReindexPolicies()
        {
            // 1. Clear existing chunks
            _db.PolicyChunks.RemoveRange(_db.PolicyChunks);
            await _db.SaveChangesAsync();

            // 2. Read all policy documents
            var policies = await _db.PolicyDocuments.AsNoTracking().ToListAsync();

            foreach (var policy in policies)
            {
                // 3. Break policy text into chunks
                var chunks = ChunkText(policy.ContentText);

                foreach (var chunk in chunks)
                {
                    // 4. Generate embedding for the chunk
                    float[] embedding = await _embeddingService.GenerateEmbedding(chunk);

                    // 5. Store chunk + embedding
                    var policyChunk = new PolicyChunk
                    {
                        PolicyId = policy.Id,
                        ChunkText = chunk,
                        EmbeddingVector = SerializeEmbedding(embedding)
                    };

                    _db.PolicyChunks.Add(policyChunk);
                }
            }

            // 6. Save all chunks
            await _db.SaveChangesAsync();

            return Ok("Policies reindexed successfully");
        }


        [HttpGet]
        public async Task<IActionResult> GetPoliciesAsync()
        {
            return Ok(await _db.PolicyDocuments.AsNoTracking().ToListAsync());
        }

        private List<string> ChunkText(string content)
        {
            return content
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim() + ".")
                .ToList();
        }

        private byte[] SerializeEmbedding(float[] embedding)
        {
            var bytes = new byte[embedding.Length * sizeof(float)];
            Buffer.BlockCopy(embedding, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
