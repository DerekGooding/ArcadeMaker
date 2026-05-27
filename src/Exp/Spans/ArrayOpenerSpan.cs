namespace Exp.Spans;

internal class ArrayOpenerSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "[";

    internal ArrayOpenerSpan() : base(Symbol)
    {
    }
}
