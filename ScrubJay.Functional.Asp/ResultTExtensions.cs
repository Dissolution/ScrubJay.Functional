using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ScrubJay.Functional.Asp;

/// <summary>
/// Extensions on <see cref="Result{T}"/>
/// </summary>
public static class ResultTExtensions
{
    extension<T>(Result<T> result)
    {
        /// <summary>
        /// Converts a <see cref="Result{T}"/> into an <see cref="IActionResult"/>
        /// </summary>
        public IActionResult ToIActionResult()
        {
            if (result.IsOk(out var value, out var error))
            {
                if (value is IStatusCodeActionResult iscar)
                    return new StatusCodeResult(iscar.StatusCode ?? ProblemDetailsHelper.DefaultOkStatusCode);
                if (value is IActionResult iar)
                    return iar;
                return new OkObjectResult(value);
            }
            else
            {
                var problem = ProblemDetailsHelper.GetProblemDetails(error);
                return new ObjectResult(problem)
                {
                    StatusCode = problem.Status ?? ProblemDetailsHelper.DefaultFailStatusCode,
                };
            }
        }
        
        public ActionResult<T> ToActionResult()
        {
            if (result.IsOk(out var value, out var error))
            {
                if (value is ActionResult<T> actionResult)
                    return actionResult;
                return new ActionResult<T>(value);
            }
            else
            {
                var problem = ProblemDetailsHelper.GetProblemDetails(error);
                return new ObjectResult(problem)
                {
                    StatusCode = problem.Status ?? ProblemDetailsHelper.DefaultFailStatusCode,
                };
            }
        }
        
        public IResult ToIResult()
        {
            if (result.IsOk(out var value, out var error))
            {
                if (value is IResult ir)
                    return ir;
                return Results.Ok(value);
            }
            else
            {
                var problem = ProblemDetailsHelper.GetProblemDetails(error);
                return Results.Problem(problem);
            }
        }
    }
}