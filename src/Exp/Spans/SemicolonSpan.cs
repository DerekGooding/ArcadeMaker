namespace Exp.Spans;

internal class SemicolonSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = ";";

    internal SemicolonSpan() : base(Symbol)
    {
    }
}
