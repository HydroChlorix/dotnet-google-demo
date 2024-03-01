using Demo.WorkerService;
using Demo.WorkerService.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<PubSubOption>(builder.Configuration.GetSection("PubSub"));

var host = builder.Build();
host.Run();
