using Microsoft.AspNetCore.Mvc;

namespace ScrubJay.Functional.Asp;

public static class ControllerExtensions
{
    extension(ControllerBase controller)
    {
        public ActionResult FromResult(Result result)
        {
            if (!result.IsError(out var ex))
            {
                return new OkResult();
            }
            else
            {
                int? statusCode = ProblemDetailsHelper.GetHttpStatusCode(ex);

                var problemDetails = controller.ProblemDetailsFactory
                    .CreateProblemDetails(
                        httpContext: controller.HttpContext,
                        statusCode: statusCode,
                        title: ex.Message ?? "<unknown error>",
                        type: ex.GetType().Name,
                        detail: ProblemDetailsHelper.GetStackTrace(ex));

                return new ObjectResult(problemDetails)
                {
                    StatusCode = statusCode,
                };
            }
        }

        public ActionResult<T> FromResult<T>(Result<T> result)
        {
            if (result.IsOk(out var value, out var ex))
            {
                if (value is ActionResult<T> art)
                    return art;
                if (value is ActionResult ar)
                    return ar;
                return new ActionResult<T>(value);
            }
            else
            {
                var problemDetails = ProblemDetailsHelper.GetProblemDetails(ex);

                return controller.Problem(
                    detail: problemDetails.Detail,
                    instance: problemDetails.Instance,
                    statusCode: problemDetails.Status,
                    title: problemDetails.Title,
                    type: problemDetails.Type,
                    extensions: problemDetails.Extensions);
            }
        }
        
        public ActionResult<T> FromResult<T,E>(Result<T,E> result)
        {
            if (result.IsOk(out var value, out var error))
            {
                if (value is ActionResult<T> art)
                    return art;
                if (value is ActionResult ar)
                    return ar;
                return new ActionResult<T>(value);
            }
            else
            {
                if (error is ActionResult<T> art)
                    return art;
                
                if (error is ActionResult ar)
                    return ar;
                
                var problemDetails = ProblemDetailsHelper.GetProblemDetails<E>(error);

                return controller.Problem(
                    detail: problemDetails.Detail,
                    instance: problemDetails.Instance,
                    statusCode: problemDetails.Status,
                    title: problemDetails.Title,
                    type: problemDetails.Type,
                    extensions: problemDetails.Extensions);
            }
        }
        
        
        
    }
}