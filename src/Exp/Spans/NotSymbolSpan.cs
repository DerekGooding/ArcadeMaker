namespace Exp.Spans;

internal class NotSymbolSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "!";

    internal NotSymbolSpan() : base(Symbol)
    {
    }
}
