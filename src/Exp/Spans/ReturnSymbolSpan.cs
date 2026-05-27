namespace Exp.Spans;

internal class ReturnSymbolSpan : Span, ISymbol
{
    public static string Symbol { get; } = "=>";

    internal ReturnSymbolSpan() : base(Symbol)
    {
    }
}
