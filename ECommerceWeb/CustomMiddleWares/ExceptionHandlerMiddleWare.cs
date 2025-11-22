using E_Commerce.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.CustomMiddleWares
{
    public class ExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleWare> logger;

        public ExceptionHandlerMiddleWare(RequestDelegate Next, ILogger<ExceptionHandlerMiddleWare> logger)
        {
            next = Next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next.Invoke(httpContext);

                await HandleNotFoundEndPointAsync(httpContext);
            }
            catch (Exception ex)
            {
                //logging
                logger.LogError(ex, "Something Went Wrong");

                //return custom error response
                var Problem = new ProblemDetails()
                {
                    Title = "Un Unexpected Error Occured",
                    Detail = ex.Message,
                    Instance = httpContext.Request.Path,
                    Status = ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    }
                };

                httpContext.Response.StatusCode = Problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(Problem);
            }
        }

        private static async Task HandleNotFoundEndPointAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var Problem = new ProblemDetails()
                {
                    Title = "error  processing the http request , EndPoint Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Endpoint {httpContext.Request.Path} Not Found",
                    Instance = httpContext.Request.Path
                };
                await httpContext.Response.WriteAsJsonAsync(Problem);
            }
        }
    }
}
