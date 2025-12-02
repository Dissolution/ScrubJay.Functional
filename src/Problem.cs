// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScrubJay.Functional;

/// <summary>
/// A generic Problem for use as a non-<see cref="Exception"/> Error for a <see cref="Result{T,E}"/>
/// that is a rough approximation of <see href="https://www.rfc-editor.org/rfc/rfc9457.html">Problem Details</see>
/// </summary>
public record class Problem
{
    public string? Title { get; set; }
    
    public string? Detail { get; set; }
    
    public Exception? Exception { get; set; }
    
    public Dictionary<string, object?> Data { get; } = new(StringComparer.OrdinalIgnoreCase);

    
    public Problem() { }
    
    public Problem(string? title, string? detail = null, Exception? exception = null)
    {
        this.Title = title;
        this.Detail = detail;
        this.Exception = exception;        
    }
}
