namespace Exp.Spans;

internal class PrintWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "print";

    internal PrintWordSpan() : base(Keyword)
    {
    }
}
