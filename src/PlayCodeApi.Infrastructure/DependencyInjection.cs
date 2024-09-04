using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayCodeApi.Application;
using PlayCodeApi.Contract.V1;

namespace PlayCodeApi.Infrastructure;

public static class DependencyInjection
{
    public static WebApplication AddPlayCodeApi(this WebApplication app)
    {
        /****************************************************
         * PlayCodes API
         ****************************************************/

        app.MapGet(ApiRoutes.PlayCodes.Get, (string code, IPlayCodeApiHandler handler) 
                => handler.GetPlayCodeAsync(code))
            .WithName("GetPlayCode").WithOpenApi()
            .WithSummary("Get a play code by its unique code")
            .Produces<PlayCodeData>().Produces<ProblemDetails>(404);

        app.MapPost(ApiRoutes.PlayCodes.Purchase, ([FromBody] PlayCodePurchase purchase, IPlayCodeApiHandler handler) 
                => handler.CreatePlayCodeAsync(purchase.Amount))
            .WithName("CreatePlayCode").WithOpenApi()
            .WithSummary("Create a playcode with a given amount")
            .Produces<PlayCodeData>().Produces<ProblemDetails>(400);

        app.MapPost(ApiRoutes.PlayCodes.TopUp, ([FromRoute] string code, [FromBody] PlayCodeTopUp purchase, IPlayCodeApiHandler handler) 
                => handler.TopUpPlayCodeAsync(code, purchase.Amount))
            .WithName("TopUpPlayCode").WithOpenApi()
            .WithSummary("Top up a playcode with a given amount")
            .Produces<PlayCodeData>().Produces<ProblemDetails>(400).Produces<ProblemDetails>(404);

        app.MapPost(ApiRoutes.PlayCodes.CashOut, (string code, IPlayCodeApiHandler handler) 
                => handler.CashOutPlayCodeAsync(code))
            .WithName("CashOutPlayCode").WithOpenApi()
            .WithSummary("Cashout playcode. The playcode is considered 'closed' and cannot be used again.")
            .Produces<CashoutResult>().Produces<ProblemDetails>(400).Produces<ProblemDetails>(404);
        
        return app;
    }
}