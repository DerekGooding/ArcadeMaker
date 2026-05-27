namespace Exp.Spans;

internal class FalseWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "false";

    internal FalseWordSpan() : base(Keyword)
    {
    }
}
