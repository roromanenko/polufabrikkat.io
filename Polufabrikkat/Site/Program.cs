using Polufabrikkat.Site;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSiteServices(builder.Environment);

var app = builder.Build();
app.UseSiteServices(builder.Environment);
app.Run();
