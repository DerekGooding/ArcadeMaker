namespace Exp.Spans;

internal class BaseArrayWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "basearray";

    internal BaseArrayWordSpan() : base(Keyword)
    {
    }
}
