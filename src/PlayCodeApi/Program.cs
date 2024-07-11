using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlayCodeApi.PlayCodes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});

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

var playCodes = new InMemoryPlayCodesRepository();

/****************************************************
 * PlayCodes API
 ****************************************************/

app.MapGet("/playcodes/{code}", (string code) => playCodes.GetPlayCodeAsync(code))
    .WithName("GetPlayCode").WithOpenApi()
    .WithSummary("Get a play code by its unique code")
    .Produces<PlayCode>().Produces<ProblemDetails>(404);

app.MapPost("/playcodes/purchase", ([FromBody] PlayCodePurchase purchase) => playCodes.CreatePlayCodeAsync(purchase.Amount))
    .WithName("CreatePlayCode").WithOpenApi()
    .WithSummary("Create a playcode with a given amount")
    .Produces<PlayCode>().Produces<ProblemDetails>(400);

app.MapPost("/playcodes/{code}/topup", ([FromRoute] string code, [FromBody] PlayCodeTopUp purchase) => 
    playCodes.TopUpPlayCodeAsync(code, purchase.Amount))
    .WithName("TopUpPlayCode").WithOpenApi()
    .WithSummary("Top up a playcode with a given amount")
    .Produces<PlayCode>().Produces<ProblemDetails>(400).Produces<ProblemDetails>(404);

app.MapPost("/playcodes/{code}/cashout", (string code) => playCodes.CashOutPlayCodeAsync(code))
    .WithName("CashOutPlayCode").WithOpenApi()
    .WithSummary("Cashout playcode. The playcode is considered 'closed' and cannot be used again.")
    .Produces<CashoutResult>().Produces<ProblemDetails>(400).Produces<ProblemDetails>(404);

app.Run();