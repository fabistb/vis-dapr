using Dapr.AI.Conversation.Extensions;
using Main.Actors;
using Main.Infrastructure;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.RegisterDaprClientAndSecretStore();

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddDaprClient();
builder.Services.AddControllers().AddDapr();
builder.Services.AddHealthChecks();

builder.Services.AddTransient<IActorFactory<IMathActor>, ActorFactory<IMathActor>>();

builder.Services.AddTransient<IPubSubService, PubSubService>();
builder.Services.AddTransient<IStateStoreService, StateStoreService>();
builder.Services.AddTransient<ISecretStoreService, SecretStoreService>();
builder.Services.AddTransient<IServiceInvocationService, ServiceInvocationService>();
builder.Services.AddTransient<IBlobStorageService, BlobStorageService>();
builder.Services.AddTransient<IServiceBusQueueService, ServiceBusQueueService>();
builder.Services.AddTransient<IConfigurationService, ConfigurationService>();
builder.Services.AddTransient<ILockService, LockService>();
builder.Services.AddTransient<IResiliencyService, ResiliencyService>();
builder.Services.AddTransient<ICryptographyService, CryptographyService>();
builder.Services.AddTransient<IConversationService, ConversationService>();

builder.Services.AddDaprConversationClient();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<MathActor>();
});

builder.Services.AddApiVersioning(v =>
{
    v.ReportApiVersions = true;
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.DefaultApiVersion = new ApiVersion(1, 0);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDaprSidekick(builder.Configuration);
}

// Configure the HTTP request pipeline
var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCloudEvents();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health");
    endpoints.MapControllers();
    endpoints.MapSubscribeHandler();
    endpoints.MapActorsHandlers();
});

app.MapMethods("/servicebusqueue", new[] {"OPTIONS"}, () => "servicebusqueue input binding is available");

app.Run();