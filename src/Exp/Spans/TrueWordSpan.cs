namespace Exp.Spans;

internal class TrueWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "true";

    internal TrueWordSpan() : base(Keyword)
    {
    }
}
