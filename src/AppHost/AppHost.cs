var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Api>("api");

builder.AddProject<Projects.A2A_Server>("a2a-server");

builder.Build().Run();
