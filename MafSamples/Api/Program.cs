using Api.Common.Api;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Samples.AGUIStateSnapShot;
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

await app.MapAGUISnapShotExample();

var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

var agent = await ManualToolCallExtensions.CreateAgent(agentFactory);

app.MapAGUI(Routes.ManualToolCallRoute, agent);

app.Run();

