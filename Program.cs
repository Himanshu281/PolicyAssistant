using Microsoft.EntityFrameworkCore;
using PolicyAssistant.Services.Embeddings;
using PolicyAssistant.Services.LLM;
using PolicyAssistant.Services.Similarity;
using PolicyAssitant.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>();
builder.Services.AddHttpClient<ILLMService, OllamaLLMService>();
builder.Services.AddSingleton<CosineSimilarityService>();

// Register EF Core DbContext using SQL Server LocalDB
var conn = builder.Configuration.GetConnectionString("PolicyAssitantConnection")
           ?? "Server=(localdb)\\MSSQLLocalDB;Database=PolicyAssitant;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<PolicyDbContext>(options =>
    options.UseSqlServer(conn));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
