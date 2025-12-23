using System;

namespace PolicyAssitant.Models;

public class AIQueryLog
{
    // Maps to database column "QueryId"
    public int Id { get; set; }

    public string UserQuestion { get; set; } = string.Empty;

    public string RetrievedPolicyText { get; set; } = string.Empty;

    public string AIResponse { get; set; } = string.Empty;

    public double ConfidenceScore { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}