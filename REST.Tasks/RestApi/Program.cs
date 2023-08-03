using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers((options) =>
{
    options.CacheProfiles.Add("Default", new CacheProfile { Duration = 10 });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
