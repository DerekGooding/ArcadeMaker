namespace Exp.Spans;

internal class ThrowWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "throw";

    internal ThrowWordSpan() : base(Keyword)
    {
    }
}
