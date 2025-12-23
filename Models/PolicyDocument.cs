namespace PolicyAssitant.Models;

public class PolicyDocument
{
    // Maps to database column "PolicyId"
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    // Maps to "ContentText"
    public string ContentText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<PolicyChunk> Chunks { get; set; } = new List<PolicyChunk>();
}