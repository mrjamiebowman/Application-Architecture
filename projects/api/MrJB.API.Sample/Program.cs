using MrJB.API.Sample;
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

builder.Services.AddControllers();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
} catch (Exception)
{
    throw;
}

