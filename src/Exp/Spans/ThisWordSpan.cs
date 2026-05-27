namespace Exp.Spans;

internal class ThisWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "this";

    internal ThisWordSpan() : base(Keyword)
    {
    }
}
