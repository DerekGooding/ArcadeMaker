namespace Exp.Spans;

internal class SourceCloserSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "}";

    internal SourceCloserSpan() : base(Symbol)
    {
    }
}
