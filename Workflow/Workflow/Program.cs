using Asp.Versioning;
using Workflow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddOrderWorkflows();

builder.Services.AddApiVersioning(v =>
{
    v.ReportApiVersions = true;
    v.ApiVersionReader = new UrlSegmentApiVersionReader();
    v.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc();

var app = builder.Build();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
