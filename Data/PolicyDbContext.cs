using Microsoft.EntityFrameworkCore;
using PolicyAssitant.Models;

namespace PolicyAssitant.Data;

public class PolicyDbContext : DbContext
{
    public PolicyDbContext(DbContextOptions<PolicyDbContext> options) : base(options) { }

    public DbSet<PolicyDocument> PolicyDocuments { get; set; } = null!;
    public DbSet<PolicyChunk> PolicyChunks { get; set; } = null!;
    public DbSet<AIQueryLog> AIQueryLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table names to match your existing DB
        modelBuilder.Entity<PolicyDocument>().ToTable("PolicyDocuments");
        modelBuilder.Entity<PolicyChunk>().ToTable("policychunks");
        modelBuilder.Entity<AIQueryLog>().ToTable("AIQueryLog");

        // Column name mappings for existing columns (PolicyId, ChunkId, QueryId, ContentText, etc.)
        modelBuilder.Entity<PolicyDocument>()
            .Property(p => p.Id)
            .HasColumnName("PolicyId");

        modelBuilder.Entity<PolicyDocument>()
            .Property(p => p.ContentText)
            .HasColumnName("ContentText");

        modelBuilder.Entity<PolicyChunk>()
            .Property(c => c.Id)
            .HasColumnName("ChunkId");

        modelBuilder.Entity<PolicyChunk>()
            .Property(c => c.EmbeddingVector)
            .HasColumnType("varbinary(max)")
            .HasColumnName("EmbeddingVector");

        modelBuilder.Entity<AIQueryLog>()
            .Property(q => q.Id)
            .HasColumnName("QueryId");

        // Relationship: policychunks.PolicyId -> PolicyDocuments.PolicyId
        modelBuilder.Entity<PolicyChunk>()
            .HasOne(c => c.PolicyDocument)
            .WithMany(d => d.Chunks)
            .HasForeignKey(c => c.PolicyId)
            .HasPrincipalKey(d => d.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Example: add an index for faster text search/lookup (optional)
        modelBuilder.Entity<PolicyChunk>()
            .HasIndex(c => c.PolicyId)
            .HasDatabaseName("IX_PolicyChunks_PolicyId");
    }
}