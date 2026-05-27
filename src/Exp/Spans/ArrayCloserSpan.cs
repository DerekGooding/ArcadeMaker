namespace Exp.Spans;

internal class ArrayCloserSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "]";

    internal ArrayCloserSpan() : base(Symbol)
    {
    }
}
