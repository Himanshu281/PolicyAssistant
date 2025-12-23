namespace PolicyAssitant.Models;

public class PolicyChunk
{
    public int Id { get; set; }
    public int PolicyId { get; set; }

    // Chunk text extracted from a policy document
    public string ChunkText { get; set; } = string.Empty;

    // Embedding vector stored as binary (matches hex display like 0x01AF in SSMS)
    public byte[]? EmbeddingVector { get; set; }

    // Navigation
    public PolicyDocument? PolicyDocument { get; set; }
}