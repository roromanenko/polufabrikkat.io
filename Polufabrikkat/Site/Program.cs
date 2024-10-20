using Polufabrikkat.Site;
using Polufabrikkat.Core;
using Microsoft.Extensions.Hosting;
using Polufabrikkat.Site.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddSiteServices(builder.Environment, builder.Configuration);

builder.Services.AddHostedService<DelayedPublicationService>();

var app = builder.Build();

app.UseSiteServices(builder.Environment);

app.Run();
