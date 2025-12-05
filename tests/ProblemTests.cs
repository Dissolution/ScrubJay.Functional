namespace ScrubJay.Functional.Tests;

public class ProblemTests
{
    [Fact]
    public void FluentDataAddWorks()
    {
        Problem problem = new Problem("something went terribly wrong")
        {
            { "event_id", 147 },
        };
        Assert.Single(problem.Data);
        Assert.True(problem.Data.TryGetValue("event_id", out var eventId));
        Assert.True(eventId is int);
        var eid = (int)eventId;
        Assert.Equal(147, eid);
    }
}