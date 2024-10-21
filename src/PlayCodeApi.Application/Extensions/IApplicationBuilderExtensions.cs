using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PlayCodeApi.Contract.V1;

namespace PlayCodeApi.Application.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePlayCodeExceptionHandler(this IApplicationBuilder app)
    {
        // Return custom status code and include exception so it can be handled on client side
        app.UseExceptionHandler(x =>
            x.Run(async context =>
            {
                var ex = context.Features.Get<IExceptionHandlerPathFeature>();
                var extensions = new Dictionary<string, object>();
                var statusCode = context.Response.StatusCode;

                if (ex.Error is PlayCodeException playCodeException)
                {
                    statusCode = (int)playCodeException.StatusCode;
                    extensions["playcode"] = playCodeException.PlayCode;
                    extensions["playcode-system-id"] = playCodeException.SystemId;
                    extensions["playcode-error-code"] = playCodeException.ErrorCode;
                }
                
                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = ReasonPhrases.GetReasonPhrase(statusCode),
                    Detail = ex.Error.Message,
                    Extensions = extensions!
                };
        
                await Results.Problem(problemDetails).ExecuteAsync(context);
            }));
        return app;
    }
}