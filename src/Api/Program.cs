using A2A.Client.Services;
using A2A.Client.Settings;
using Api;
using Shared.Agents;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<LanguageModelSettings>(settings =>
    builder.Configuration.GetSection(nameof(LanguageModelSettings)).Bind(settings));

builder.Services.Configure<A2ADiscoverySettings>(settings =>
    builder.Configuration.GetSection(nameof(A2ADiscoverySettings)).Bind(settings));

builder.Services.AddSingleton<IAgentFactory, AgentFactory>();
builder.Services.AddSingleton<IA2AAgentDiscoveryService, A2AAgentDiscoveryService>();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.MapSamples();

app.Run();

