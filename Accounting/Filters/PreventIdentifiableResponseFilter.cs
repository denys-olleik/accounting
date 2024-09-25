using Accounting.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Filters
{
  public class PreventIdentifiableResponseFilter : IActionFilter
  {
    public void OnActionExecuting(ActionExecutingContext context)
    {
      // No action needed before execution
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
      if (context.Result is ObjectResult objectResult &&
          objectResult.Value != null)
      {
        var type = objectResult.Value.GetType();
        var interfaces = type.GetInterfaces();
        var identifiableInterface = interfaces.FirstOrDefault(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IIdentifiable<>));

        if (identifiableInterface != null)
        {
          context.Result = new BadRequestObjectResult("Returning types implementing IIdentifiable is not allowed.");
        }
      }
    }
  }
}