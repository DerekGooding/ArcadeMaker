namespace Exp.Spans;

internal class ContinueWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "continue";

    internal ContinueWordSpan() : base(Keyword)
    {
    }
}
