namespace Exp.Spans;

internal class StaticWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "static";

    internal StaticWordSpan() : base(Keyword)
    {
    }
}
