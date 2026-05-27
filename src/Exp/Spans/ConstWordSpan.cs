namespace Exp.Spans;

internal class ConstWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "const";

    internal ConstWordSpan() : base(Keyword)
    {
    }
}
