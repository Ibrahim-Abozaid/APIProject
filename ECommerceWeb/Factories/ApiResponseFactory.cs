using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.Factories
{
    public static class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationResponse(ActionContext actionContext)
        {
            var errors = actionContext.ModelState.Where(x => x.Value.Errors.Count > 0)
                                                       .ToDictionary(x => x.Key,
                                                       x => x.Value.Errors.Select(x => x.ErrorMessage).ToArray());

            var Problem = new ProblemDetails()
            {
                Title = "Validation Error",
                Detail = "One or More validation errors occurred",
                Status = StatusCodes.Status404NotFound,
                Extensions =
                        {
                            {"Errors" , errors }
                        }
            };
            return new BadRequestObjectResult(Problem);
        }
    }
}
