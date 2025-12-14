using DataCaching.RestApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Controllers
{
  public abstract class BaseApiController : ControllerBase
  {
    public override ObjectResult Problem(
      string? detail = null,
      string? instance = null,
      int? statusCode = null,
      string? title = null,
      string? type = null,
      IDictionary<string, object?>? extensions = null)
    {
      var problemDetails = new ErrorResponseDetails
      {
        Detail = detail,
        Instance = instance,
        Status = statusCode ?? 500,
        Title = title,
        Type = type,
      };

      if (extensions is not null)
      {
        foreach (var extension in extensions)
        {
          problemDetails.Extensions.Add(extension);
        }
      }

      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }
  }
}
