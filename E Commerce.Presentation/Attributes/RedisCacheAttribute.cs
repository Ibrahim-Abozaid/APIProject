using E_Commerce.services_Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Presentation.Attributes
{
    internal class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int durationInMin;

        public RedisCacheAttribute(int DurationInMin = 5)
        {
            durationInMin = DurationInMin;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // steps :
            // Get Cach response from Denpendency injection container
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var CacheKey = CreateCacheKey(context.HttpContext.Request);
            // Check if cache data exist --  (note : key => request , value => data)
            var CacheValue = await CacheService.GetAsync(CacheKey);

            // if exists return cached data and skip executing of endpoint (will check with key if data(value) is exist)
            if (CacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = CacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }
            // if not exist execute endpoint and store the result in cache if 200 ok response
            var ExecutedContext = await next.Invoke();
            if (ExecutedContext.Result is OkObjectResult result)
            {
                await CacheService.SetAsync(CacheKey, result.Value, TimeSpan.FromMinutes(durationInMin));
            }
            // note : key => request , value => data

        }

        // /api/Products
        // /api/Products?brandId=2&typeId=1
        // /api/Products?brandId=2
        // /api/Products?typeId=1&brandId=2
        // /api/Products?typeId=1

        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder key = new StringBuilder();
            key.Append(request.Path);// /api/Products
            foreach (var item in request.Query.OrderBy(x => x.Key)) // /api/Products|brandId-2|typeId-1
                key.Append($"{item.Key}-{item.Value}");

            return key.ToString();

        }
    }
}
