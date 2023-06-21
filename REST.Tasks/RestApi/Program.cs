using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseIIS();

builder.Services.AddControllers((options) =>
{
    options.CacheProfiles.Add("Default", new CacheProfile { Duration = 10 });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
