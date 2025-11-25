using System.Collections.Concurrent;
using System.Security;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ScrubJay.Functional.Asp;

public static class ErrorProblemDetailsHelper
{
    private static readonly Dictionary<Type, int> _exceptionStatusCodes;
    private static readonly ConcurrentDictionary<Type, Func<Exception, ProblemDetails>> _exceptionTypeProblemDetailsFactoryMap = [];


    static ErrorProblemDetailsHelper()
    {
        _exceptionStatusCodes = new Dictionary<Type, int>
        {
            // 400+
            [typeof(ArgumentException)] = StatusCodes.Status400BadRequest,
            [typeof(FormatException)] = StatusCodes.Status400BadRequest,
            [typeof(AuthenticationException)] = StatusCodes.Status401Unauthorized,
            [typeof(UnauthorizedAccessException)] = StatusCodes.Status403Forbidden,
            [typeof(SecurityException)] = StatusCodes.Status403Forbidden,
            [typeof(KeyNotFoundException)] = StatusCodes.Status404NotFound,
            [typeof(FileNotFoundException)] = StatusCodes.Status404NotFound,
            [typeof(NotSupportedException)] = StatusCodes.Status405MethodNotAllowed,
            [typeof(TimeoutException)] = StatusCodes.Status408RequestTimeout,
            [typeof(InvalidOperationException)] = StatusCodes.Status409Conflict,
            [typeof(OperationCanceledException)] = 499,
            [typeof(TaskCanceledException)] = 499,
        };
    }

    private static ProblemDetails ToProblemDetails(Exception exception)
    {
        Type exceptionType = exception.GetType();
        int statusCode = _exceptionStatusCodes.GetValueOrDefault(exceptionType, StatusCodes.Status500InternalServerError);
        
        var problemDetails = new ProblemDetails
        {
            Type = exceptionType.Name,
            Title = exception.Message,
            Detail = exception.StackTrace,
            Status = statusCode,
        };
        return problemDetails;
    }

    public static void AddProblemDetailsFactory<X>(Func<X, ProblemDetails> factory)
        where X : Exception
    {
        _exceptionTypeProblemDetailsFactoryMap[typeof(X)] = (Func<Exception, ProblemDetails>)factory;
    }

    public static ActionResult ToActionResult(Exception exception)
    {
        var exceptionType = exception.GetType();
        ProblemDetails problemDetails;
        
        if (_exceptionTypeProblemDetailsFactoryMap.TryGetValue(exceptionType, out var factory))
        {
            problemDetails = factory(exception);
        }
        else
        {
            problemDetails = ToProblemDetails(exception);
        }
        
        ObjectResult objectResult = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
        };
        return objectResult;
    }

    // public static IResult ToIResult(Exception exception)
    // {
    //     var exceptionType = exception.GetType();
    //     ProblemDetails problemDetails;
    //     
    //     if (_exceptionTypeProblemDetailsFactoryMap.TryGetValue(exceptionType, out var factory))
    //     {
    //         problemDetails = factory(exception);
    //     }
    //     else
    //     {
    //         problemDetails = ToProblemDetails(exception);
    //     }
    //
    //     var problem = Results.Problem(problemDetails);
    //     return problem;
    // }
}