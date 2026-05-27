namespace Exp.Spans;

internal class DoSymbolSpan : Span, ISymbol
{
    public static string Symbol { get; } = "->";

    internal DoSymbolSpan() : base(Symbol)
    {
    }
}
