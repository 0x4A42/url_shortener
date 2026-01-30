using UrlShortener;
using UrlShortener.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<UrlCollection>();
var app = builder.Build();
app.MapEndpoints();
app.Run();
