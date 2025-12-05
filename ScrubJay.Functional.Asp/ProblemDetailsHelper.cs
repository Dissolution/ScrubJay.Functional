using System.Diagnostics;
using System.Security;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ScrubJay.Functional.Asp;

public enum StackTraceLevel
{
    None,
    Sanitized,
    Full,
}

public static class ProblemDetailsHelper
{
    private static readonly Dictionary<Type, int> _exceptionStatusCodes;

    public static int DefaultOkStatusCode { get; set; } = StatusCodes.Status200OK;

    public static int DefaultFailStatusCode { get; set; } = StatusCodes.Status500InternalServerError;

    public static StackTraceLevel StackTraceLevel { get; set; } = StackTraceLevel.None;

    static ProblemDetailsHelper()
    {
        _exceptionStatusCodes = new Dictionary<Type, int>
        {
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
            [typeof(OperationCanceledException)] = StatusCodes.Status499ClientClosedRequest,
            [typeof(TaskCanceledException)] = StatusCodes.Status499ClientClosedRequest,
            [typeof(ArgumentNullException)] = StatusCodes.Status500InternalServerError,
            [typeof(NullReferenceException)] = StatusCodes.Status500InternalServerError,
        };
    }

    private static string GetSanitizedStackTrace(Exception error)
    {
        var trace = new StackTrace(error, true);
        var frames = trace.GetFrames();

        StringBuilder builder = new();
        for (var i = 0; i < frames.Length; i++)
        {
            var frame = frames[i];
            if (i > 0)
                builder.AppendLine();
            builder.Append(" - ");
            var method = frame.GetMethod();
            if (method is null)
            {
                builder.Append("<unknown method>");
            }
            else
            {
                var declarer = method.DeclaringType ?? method.ReflectedType ?? method.Module.GetType();
                builder.Append(declarer.Name)
                    .Append('.')
                    .Append(method.Name);
            }

            var line = frame.GetFileLineNumber();
            builder.Append(':')
                .Append(line);
        }

        return builder.ToString();
    }

    public static int GetHttpStatusCode(Exception? exception)
    {
        Type exceptionType = exception?.GetType() ?? typeof(Exception);
        int statusCode = _exceptionStatusCodes.GetValueOrDefault(exceptionType, DefaultFailStatusCode);
        return statusCode;
    }

    public static string? GetStackTrace(Exception? exception)
    {
        if (exception is null)
            return null;
        return StackTraceLevel switch
        {
            StackTraceLevel.Sanitized => GetSanitizedStackTrace(exception),
            StackTraceLevel.Full => exception.StackTrace,
            _ => null,
        };
    }
    
    public static ProblemDetails GetProblemDetails(Exception? exception)
    {
        Type exceptionType = exception?.GetType() ?? typeof(Exception);
        ProblemDetails problem;

        if (exception is null)
        {
            problem = new()
            {
                Type = exceptionType.Name,
                Title = "Unknown Error",
                Detail = "Unknown Error",
                Status = DefaultFailStatusCode,
            };
        }
        else
        {
            int statusCode = _exceptionStatusCodes.GetValueOrDefault(exceptionType, DefaultFailStatusCode);

            string? detail = StackTraceLevel switch
            {
                StackTraceLevel.Sanitized => GetSanitizedStackTrace(exception),
                StackTraceLevel.Full => exception.StackTrace,
                _ => null,
            };

            string? instance = null;
            if (!string.IsNullOrEmpty(exception.HelpLink) &&
                Uri.TryCreate(exception.HelpLink, UriKind.Absolute, out var uri))
            {
                instance = uri.ToString();
            }

            problem = new ProblemDetails
            {
                Type = exceptionType.Name,
                Title = exception.Message,
                Detail = detail,
                Status = statusCode,
                Instance = instance,
            };
        }

        return problem;
    }

    public static ProblemDetails GetProblemDetails<E>(E? error)
    {
        return error switch
        {
            Problem problem => problem.ToProblemDetails(),
            ProblemDetails details => details,
            _ => new ProblemDetails
            {
                Title = "Error", 
                Type = (error?.GetType() ?? typeof(E)).Name, 
                Detail = error?.ToString(),
            },
        };
    }
}