using AGUI.StateSnapShotEvents;
using Api;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Shared.Agents;
using Shared.Settings;
using Tools.ManualToolCall;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<LanguageModelSettings>(settings =>
    builder.Configuration.GetSection(nameof(LanguageModelSettings)).Bind(settings));

builder.Services.AddSingleton<IAgentFactory, AgentFactory>();

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


var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

var agUiAgent = await AGUISnapShotExtensions.CreateAgent(agentFactory);

app.MapAGUI(Routes.AGUISnapshotRoute, agUiAgent);

var toolCallAgent = await ManualToolCallExtensions.CreateAgent(agentFactory);

app.MapAGUI(Routes.ManualToolCallRoute, toolCallAgent);

app.Run();

