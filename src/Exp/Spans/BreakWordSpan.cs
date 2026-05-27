namespace Exp.Spans;

internal class BreakWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "break";

    internal BreakWordSpan() : base(Keyword)
    {
    }
}
