



using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace EComMSSharedLibrary.ComonResponseModel
{
    public static class ApiResponseHelper
    {
        public static IActionResult ToActionResult<T>(this ApiResponse<T> response) where T : class
        {
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => new OkObjectResult(response),
                StatusCodes.Status201Created => new CreatedResult(string.Empty, response),
                StatusCodes.Status400BadRequest => new BadRequestObjectResult(response),
                StatusCodes.Status401Unauthorized => new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized },
                StatusCodes.Status403Forbidden => new ForbidResult(),
                StatusCodes.Status404NotFound => new NotFoundObjectResult(response),
                StatusCodes.Status409Conflict => new ConflictObjectResult(response),
                _ => new ObjectResult(response) { StatusCode = response.StatusCode }
            };
        }

    }
}
