using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ScrubJay.Functional.Asp;

/// <summary>
/// Extensions on <see cref="Result{T,E}"/>
/// </summary>
public static class ResultTEExtensions
{
    extension<T, E>(Result<T, E> result)
    {
        /// <summary>
        /// Converts a <see cref="Result{T,E}"/> into an <see cref="IActionResult"/>
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
                if (error is IStatusCodeActionResult iscar)
                    return new StatusCodeResult(iscar.StatusCode ?? ProblemDetailsHelper.DefaultFailStatusCode);
                if (error is IActionResult iar)
                    return iar;
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
                if (error is IStatusCodeActionResult iscar)
                    return new StatusCodeResult(iscar.StatusCode ?? ProblemDetailsHelper.DefaultFailStatusCode);
                if (error is ActionResult<T> actionResult)
                    return actionResult;
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
                if (value is IResult ir)
                    return ir;
                var problem = ProblemDetailsHelper.GetProblemDetails<E>(error);
                return Results.Problem(problem);
            }
        }
    }
}