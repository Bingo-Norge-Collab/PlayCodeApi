using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayCodeApi.Application;
using PlayCodeApi.Contract.V1;

namespace PlayCodeApi.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds the PlayCode API to the application.
    /// This can be used by the server that handles the playcodes, ie. the hall server / third party system.
    /// Integrators must create a custom implementation of IPlayCodeApiHandler and inject this to the DI container. 
    /// </summary>
    public static WebApplication AddPlayCodeApi(this WebApplication app)
    {
        /****************************************************
         * PlayCodes API
         ****************************************************/

        app.MapGet(ApiRoutes.PlayCodes.Get, ([FromRoute] int systemId, [FromRoute] int locationId, [FromRoute] string code, IPlayCodeApiHandler handler) 
                => handler.GetPlayCodeAsync(systemId, locationId, code))
            .WithName("GetPlayCode").WithOpenApi()
            .WithSummary("Get a play code by its unique code")
            .Produces<PlayCodeData>().Produces<ProblemDetails>(404);

        app.MapPost(ApiRoutes.PlayCodes.Purchase, ([FromRoute] int systemId, [FromRoute] int locationId, [FromBody] PlayCodePurchase purchase, IPlayCodeApiHandler handler) 
                => handler.CreatePlayCodeAsync(systemId, locationId, purchase.Amount))
            .WithName("CreatePlayCode").WithOpenApi()
            .WithSummary("Create a playcode with a given amount")
            .Produces<PlayCodeData>().Produces<ProblemDetails>(400);

        app.MapPost(ApiRoutes.PlayCodes.TopUp, ([FromRoute] int systemId, [FromRoute] int locationId, [FromRoute] string code, [FromBody] PlayCodeTopUp purchase, IPlayCodeApiHandler handler) 
                => handler.TopUpPlayCodeAsync(systemId, locationId, code, purchase.Amount))
            .WithName("TopUpPlayCode").WithOpenApi()
            .WithSummary("Top up a playcode with a given amount")
            .Produces<PlayCodeData>().Produces<ProblemDetails>(400).Produces<ProblemDetails>(404);

        app.MapPost(ApiRoutes.PlayCodes.CashOut, ([FromRoute] int systemId, [FromRoute] int locationId, [FromRoute] string code, IPlayCodeApiHandler handler) 
                => handler.CashOutPlayCodeAsync(systemId, locationId, code))
            .WithName("CashOutPlayCode").WithOpenApi()
            .WithSummary("Cashout playcode. The playcode is considered 'closed' and cannot be used again.")
            .Produces<CashoutResult>().Produces<ProblemDetails>(400).Produces<ProblemDetails>(404);
        
        return app;
    }
}