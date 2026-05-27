namespace Exp.Spans;

internal class DotSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = ".";

    internal DotSpan() : base(Symbol)
    {
    }
}
