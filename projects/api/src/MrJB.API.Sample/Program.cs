using MrJB.API.Sample;
using MrJB.Sample.Application;
using MrJB.Sample.Domain.Configuration;
using MrJB.Sample.Infrastructure;
using Scalar.AspNetCore;
using TerribleSettingsAuditor;

const string ProjectName = "MrJB.API.Sample";

var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
    Args = args,
    ApplicationName = ProjectName
});

/************************************/
/*          configuration           */
/************************************/

builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    //.AddUserSecrets()
    .AddEnvironmentVariables();

// azure app config

// json overrides
builder.Configuration.AddJsonOverrides(builder.Environment);

/************************************/
/*            start-up              */
/************************************/

/************************************/
/*              services            */
/************************************/

// configuration
ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
builder.Configuration.GetSection(ApplicationConfiguration.Position).Bind(applicationConfiguration);

builder.Services.Configure<ApplicationConfiguration>(builder.Configuration.GetSection(ApplicationConfiguration.Position));

// aspire service defaults: opentelemetry (traces, metrics, logs), health checks, service discovery, resilience
builder.AddServiceDefaults();

// application layer (registers the data layer too)
builder.ConfigureApplicationLayer();

// infrastructure layer: azure key vault + feature flags (IFeatureService); flags are off when KeyVault:Uri isn't set
builder.ConfigureInfrastructureLayer();

builder.Services.AddControllers();

// unhandled exceptions return RFC 7807 problem details instead of an empty 500
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

/************************************/
/*             tsa                  */
/************************************/

builder.AddTerribleSettingsAuditor();

/************************************/
/*             build                */
/************************************/

var app = builder.Build();

// tsa: runs terrible settings auditor
await app.UseTerribleSettingsAuditorAsync(args);

// openapi + scalar: /openapi/v1.json and /scalar, toggled by Application:ShowApiReference
if (app.Environment.IsDevelopment() == true || 
    app.Environment.IsEnvironment("Local") || 
    applicationConfiguration.ShowOpenApiDocs == true
) {
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// /health and /alive (development only)
app.MapDefaultEndpoints();

try
{
    app.Run();
} catch (Exception)
{
    throw;
}

