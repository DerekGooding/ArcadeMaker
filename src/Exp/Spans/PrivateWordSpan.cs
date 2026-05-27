namespace Exp.Spans;

internal class PrivateWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "private";

    internal PrivateWordSpan() : base(Keyword)
    {
    }
}
