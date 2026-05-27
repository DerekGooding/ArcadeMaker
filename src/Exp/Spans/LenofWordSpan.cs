namespace Exp.Spans;

internal class LenofWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "lenof";

    internal LenofWordSpan() : base(Keyword)
    {
    }
}
