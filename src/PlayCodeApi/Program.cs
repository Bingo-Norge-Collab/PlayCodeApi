using Microsoft.AspNetCore.Mvc;
using PlayCodeApi.PlayCodes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(x => 
        x.Run(async ctx =>  await Results.Problem().ExecuteAsync(ctx)));
}

app.UseStatusCodePages(async ctx 
    => await Results.Problem(statusCode: ctx.HttpContext.Response.StatusCode).ExecuteAsync(ctx.HttpContext));

app.UseHttpsRedirection();

var playCodes = new InMemoryPlayCodesRepository();

app.MapGet("/playcodes/{code}", (string code) => playCodes.GetPlayCodeAsync(code))
    .WithName("GetPlayCode").WithOpenApi().Produces<PlayCode>();

app.MapPost("/playcodes/purchase", ([FromBody] PlayCodePurchase purchase) => playCodes.CreatePlayCodeAsync(purchase.Amount))
    .WithName("CreatePlayCode").WithOpenApi().Produces<PlayCode>();

app.MapPost("/playcodes/{code}/topup", ([FromRoute] string code, [FromBody] PlayCodeTopUp purchase) => 
    playCodes.TopUpPlayCodeAsync(code, purchase.Amount))
    .WithName("TopUpPlayCode").WithOpenApi().Produces<PlayCode>();

app.MapPost("/playcodes/{code}/cashout", (string code) => playCodes.CashOutPlayCodeAsync(code))
    .WithName("CashOutPlayCode").WithOpenApi().Produces<decimal>();

app.Run();

record PlayCodePurchase(decimal Amount);
record PlayCodeTopUp(decimal Amount);