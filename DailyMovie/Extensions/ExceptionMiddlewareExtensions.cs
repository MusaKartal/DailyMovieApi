using DailyMovie.ErrorModel;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace DailyMovie.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(
                options =>
                {
                    options.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.ContentType = "text/html";
                        var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
                        if (null != exceptionObject)
                        {
                            var errorMessage = $"{exceptionObject.Error.Message}";
                            await context.Response.WriteAsync(errorMessage).ConfigureAwait(false);
                        }
                    });
                }
            );
        }

    }
}
