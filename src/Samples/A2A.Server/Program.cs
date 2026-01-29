using A2A.Server.Tasks;
using Shared.Agents;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<LanguageModelSettings>(settings =>
    builder.Configuration.GetSection(nameof(LanguageModelSettings)).Bind(settings));

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IA2ATaskManager, A2ATaskManager>();
builder.Services.AddSingleton<IAgentFactory, AgentFactory>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var workflowService = app.Services.GetRequiredService<IA2ATaskManager>();

app.MapA2A(workflowService.TaskManager, "/api/a2a/tasks");

app.Run();

