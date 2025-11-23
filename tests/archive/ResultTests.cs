// using System.Collections;
// using System.Reflection;
//
// namespace ScrubJay.Functional.Tests;
//
// public class ResultTests
// {
//     public static MiscTheoryData ResultsData { get; } = new MiscTheoryData()
//     {
//         Result<int, Exception?>.Ok(147),
//         Result<BindingFlags, Exception?>.Ok(BindingFlags.ExactBinding),
//         Result<Unit, Exception?>.Ok(default),
//         Result<string, Exception?>.Ok("ABC"),
//         Result<EventArgs?, Exception?>.Ok(null),
//         Result<int, Exception?>.Error(null),
//         Result<int, Exception?>.Error(new Exception("Bad")),
//         Result<BindingFlags, Exception?>.Error(null),
//         Result<BindingFlags, Exception?>.Error(new Exception("Bad")),
//         Result<Unit, Exception?>.Error(null),
//         Result<Unit, Exception?>.Error(new Exception("Bad")),
//         Result<string, Exception?>.Error(null),
//         Result<string, Exception?>.Error(new Exception("Bad")),
//         Result<EventArgs?, Exception?>.Error(null),
//         Result<EventArgs?, Exception?>.Error(new Exception("Bad")),
//     };
//
//

//
//     [Fact]
//     public void DefaultIsError()
//     {
//         Result<object?, object> result;
//
//         result = default;
//         Assert.False(result);
//
//         result = new Result<object?, object>();
//         Assert.False(result);
//         
//         result = Activator.CreateInstance<Result<object?, object?>>()!;
//         Assert.False(result);
//     }
//
//     [Fact]
//     public void OkAndErrorDoNotGetConfused()
//     {
//         // Obj?, Obj?
//         {
//             Result<object?, object?> result;
//
//             result = Result<object?, object?>.Ok(null);
//             Assert.True(result);
//
//             result = Result<object?, object?>.Ok(147);
//             Assert.True(result);
//
//             result = Result<object?, object?>.Ok(new Exception("Bad"));
//             Assert.True(result);
//
//             result = Result<object?, object?>.Error(null);
//             Assert.False(result);
//
//             result = Result<object?, object?>.Error(147);
//             Assert.False(result);
//
//             result = Result<object?, object?>.Error(new Exception("Bad"));
//             Assert.False(result);
//         }
//
//         // int?, ex?
//         {
//             Result<int?, Exception?> result;
//
//             result = Result<int?, Exception?>.Ok(null);
//             Assert.True(result);
//
//             result = Result<int?, Exception?>.Error(null);
//             Assert.False(result);
//         }
//
//         // ie, iet
//         {
//             Result<IEnumerable?, IEnumerable<int>?> result;
//
//             result = Result<IEnumerable?, IEnumerable<int>?>.Ok(null);
//             Assert.True(result);
//
//             result = Result<IEnumerable?, IEnumerable<int>?>.Ok(new int[1, 4, 7]);
//             Assert.True(result);
//
//             result = Result<IEnumerable?, IEnumerable<int>?>.Error(null);
//             Assert.False(result);
//
//             result = Result<IEnumerable?, IEnumerable<int>?>.Error(new List<int> { 1, 4, 7, });
//             Assert.False(result);
//         }
//     }
//