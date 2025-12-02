using Microsoft.AspNetCore.Mvc;

namespace ScrubJay.Functional.Asp;

public static class ProblemExtensions
{
    extension(Problem problem)
    {
        public ProblemDetails ToProblemDetails()
        {
            int status;
            if (problem.Data.TryGetValue("Status", out var statusObj) && statusObj is int)
            {
                status = (int)statusObj;
            }
            else
            {
                status = ProblemDetailsHelper.GetHttpStatusCode(problem.Exception);
            }

            string? instance = null;
            if (problem.Data.TryGetValue("Instance", out var instanceObj))
            {
                instance = instanceObj?.ToString();
            }

            var problemDetails = new ProblemDetails()
            {
                Type = problem.Exception?.GetType().Name,
                Title = problem.Title,
                Status = status,
                Detail = problem.Detail,
                Instance = instance,
            };

            var stackTrace = ProblemDetailsHelper.GetStackTrace(problem.Exception);
            if (stackTrace is not null)
            {
                problemDetails.Extensions["StackTrace"] = stackTrace;
            }
            
            foreach (var kvp in problem.Data)
            {
                if (kvp.Key.Equals("Status", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Equals("Instance", StringComparison.OrdinalIgnoreCase))
                    continue;
                problemDetails.Extensions[kvp.Key] = kvp.Value;
            }

            return problemDetails;
        }
    }
}