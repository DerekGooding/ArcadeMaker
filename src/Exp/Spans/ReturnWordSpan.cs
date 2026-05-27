namespace Exp.Spans;

internal class ReturnWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "return";

    internal ReturnWordSpan() : base(Keyword)
    {
    }
}
