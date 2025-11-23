using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ScrubJay.Functional.Asp;

public static class ResultExtensions
{
    extension(Result result)
    {
        public IActionResult ToIActionResult() => ToActionResult(result);

        public ActionResult ToActionResult()
        {
            if (!result.IsError(out var error))
            {
                return new OkResult();
            }

            return ErrorProblemDetailsHelper.ToActionResult(error);
        }

        public IResult ToIResult()
        {
            if (!result.IsError(out var error))
            {
                return Results.Ok();
            }

            return ErrorProblemDetailsHelper.ToIResult(error);
        }
    }

    extension<T>(Result<T> result)
    {
        public IActionResult ToIActionResult()
        {
            if (result.IsOk(out var ok, out var error))
            {
                return new OkObjectResult(ok);
            }

            return ErrorProblemDetailsHelper.ToActionResult(error);
        }

        public ActionResult<T> ToActionResult()
        {
            if (result.IsOk(out var ok, out var error))
            {
                return new OkObjectResult(ok);
            }

            return ErrorProblemDetailsHelper.ToActionResult(error);
        }
    }

    extension<T, E>(Result<T, E> result)
    {
        public IActionResult ToIActionResult()
        {
            if (result.IsOk(out var ok, out var error))
            {
                return new OkObjectResult(ok);
            }

            return new ObjectResult(error)
            {
                StatusCode = 500,
            };
        }
        
        public ActionResult<T> ToActionResult()
        {
            if (result.IsOk(out var ok, out var error))
            {
                return new OkObjectResult(ok);
            }

            return new ObjectResult(error)
            {
                StatusCode = 500,
            };
        }
    }
}