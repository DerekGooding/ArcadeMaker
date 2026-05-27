namespace Exp.Spans;

internal class SourceOpenerSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "{";

    internal SourceOpenerSpan() : base(Symbol)
    {
    }
}
