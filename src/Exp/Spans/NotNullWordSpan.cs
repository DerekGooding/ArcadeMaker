namespace Exp.Spans;

internal class NotNullWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "notnull";

    internal NotNullWordSpan() : base(Keyword)
    {
    }
}
