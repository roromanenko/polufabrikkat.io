using Polufabrikkat.Site;
using Polufabrikkat.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddSiteServices(builder.Environment, builder.Configuration);

var app = builder.Build();

app.UseSiteServices(builder.Environment);

app.Run();
