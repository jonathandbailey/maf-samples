using Api.Common;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<LanguageModelSettings>(settings =>
    builder.Configuration.GetSection("LanguageModelSettings").Bind(settings));

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

var agent = await agentFactory.Create();

app.MapAGUI("/", agent);

app.Run();

