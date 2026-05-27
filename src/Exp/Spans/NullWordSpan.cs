namespace Exp.Spans;

internal class NullWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "null";

    internal NullWordSpan() : base(Keyword)
    {
    }
}
