using A2A.Server.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IA2ATaskManager, A2ATaskManager>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var workflowService = app.Services.GetRequiredService<IA2ATaskManager>();

app.MapA2A(workflowService.TaskManager, "/api/a2a/travel");

app.Run();

