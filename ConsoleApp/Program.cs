using ScrubJay.Functional;
using static ScrubJay.Functional.Prelude;


var n = None;
var a = Option.NotNull(147);
var s = Option.NotNull("abc");
var s2 = Option.NotNull((int?)null);
var r = RefOption<ReadOnlySpan<char>>.Some("abc".AsSpan());
Func<string, Unit> f = str =>
{
    Console.WriteLine(str);
    return default;
};