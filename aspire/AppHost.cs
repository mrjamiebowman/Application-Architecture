using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// launch settings
var launchSettings = builder.Configuration.GetValue<string>("LAUNCH_PROFILE") ?? "DEV";

// projects
builder.AddProject<MrJB_API_Sample>("api-sample", launchSettings);

builder.Build().Run();