using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using PlayCodeApi.Application;
using PlayCodeApi.Application.Extensions;
using PlayCodeApi.Infrastructure;
using PlayCodeApi.SwaggerDemo.ApiHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<IPlayCodeApiHandler, DemoPlayCodeApiHandler>();
builder.Services.AddPlayCodeApplication();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UsePlayCodeExceptionHandler();

app.UseHttpsRedirection();

app.AddPlayCodeApi();

app.Run();