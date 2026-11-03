using TerribleSettingsAuditor;

const string ProjectName = "MrJB.API.Sample";

var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
    Args = args,
    ApplicationName = ProjectName
});

/************************************/
/*          configuration           */
/************************************/


// Add services to the container.

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

app.Run();
