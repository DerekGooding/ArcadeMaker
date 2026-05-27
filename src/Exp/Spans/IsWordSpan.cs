namespace Exp.Spans;

internal class IsWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "is";

    internal IsWordSpan() : base(Keyword)
    {
    }
}
