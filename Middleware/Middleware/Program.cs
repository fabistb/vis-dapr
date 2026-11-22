using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddDapr();
builder.Services.AddHealthChecks();

builder.Services.AddApiVersioning(v =>
{
    v.ReportApiVersions = true;
    v.ApiVersionReader = new UrlSegmentApiVersionReader();
    v.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
