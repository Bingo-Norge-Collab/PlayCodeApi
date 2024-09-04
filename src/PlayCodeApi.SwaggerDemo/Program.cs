using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using PlayCodeApi.Application;
using PlayCodeApi.Infrastructure;
using PlayCodeApi.PlayCodes;
using PlayCodeApi.SwaggerDemo.PlayCodes;

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

// Return custom status code and message for exceptions
app.UseExceptionHandler(x =>
    x.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerPathFeature>();
        var statusCode = context.Response.StatusCode;
        var detail = ex?.Error.Message ?? "An error occurred while processing your request.";
        
        // If the exception is a PlayCodeException, use the status code from the exception
        if (ex?.Error is PlayCodeException playCodeException) 
            statusCode = (int)playCodeException.StatusCode;
        
        await Results.Problem(statusCode:statusCode, detail:detail).ExecuteAsync(context);
    }));

app.UseHttpsRedirection();

app.AddPlayCodeApi();

app.Run();