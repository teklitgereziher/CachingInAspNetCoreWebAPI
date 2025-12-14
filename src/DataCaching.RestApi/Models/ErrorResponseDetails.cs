using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Models
{
  public class ErrorResponseDetails : ProblemDetails
  {
    public Dictionary<string, string[]>? Errors { get; set; }
  }
}
