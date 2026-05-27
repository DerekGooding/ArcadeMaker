namespace Exp.Spans;

internal class SetWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "var";

    internal SetWordSpan() : base(Keyword)
    {
    }
}
