using A2A.Server.Services;
using A2A.Server.Settings;
using A2A.Server.Tasks;
using Microsoft.Extensions.Options;
using Shared.Agents;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<LanguageModelSettings>(settings =>
    builder.Configuration.GetSection(nameof(LanguageModelSettings)).Bind(settings));

builder.Services.Configure<CardSettings>(settings =>
    builder.Configuration.GetSection(nameof(CardSettings)).Bind(settings));

builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IA2ATaskManager, A2ATaskManager>();
builder.Services.AddSingleton<IA2ACardService, A2ACardService>();
builder.Services.AddSingleton<IAgentFactory, AgentFactory>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var workflowService = app.Services.GetRequiredService<IA2ATaskManager>();
var cardSettings = app.Services.GetRequiredService<IOptions<CardSettings>>();
var weatherCardPath = cardSettings.Value.AgentCards.FirstOrDefault(c => c.Name == "Weather")?.Url
    ?? throw new InvalidOperationException("Weather agent card configuration not found");

app.MapA2A(workflowService.TaskManager, $"{weatherCardPath}");

app.Run();

