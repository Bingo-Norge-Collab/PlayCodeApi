using System.Reflection;
using PlayCodeApi.Application;
using PlayCodeApi.Application.Extensions;
using PlayCodeApi.Infrastructure;
using PlayCodeApi.Proxy.ApiHandler;
using PlayCodeApi.Proxy.Config;
using PlayCodeApi.Proxy.OkBingo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});
builder.Services.AddHttpClient();

var configSection = builder.Configuration.GetSection(nameof(SystemOptions));
builder.Services.Configure<SystemOptions>(configSection);

builder.Services.AddScoped<IOkBingoService, OkBingoService>();
builder.Services.AddScoped<IOkBingoApiHandler, OkBingoApiHandler>();

builder.Services.AddScoped<IHttpPlayCodeApiHandler, HttpPlayCodeApiHandler>();
builder.Services.AddScoped<IPlayCodeApiHandler, ProxyApiHandler>();
builder.Services.AddPlayCodeApplication();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UsePlayCodeExceptionHandler();

app.UseHttpsRedirection();

app.AddPlayCodeApi();

app.Run();