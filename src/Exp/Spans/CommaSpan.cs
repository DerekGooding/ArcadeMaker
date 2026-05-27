namespace Exp.Spans;

internal class CommaSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = ",";

    internal CommaSpan() : base(Symbol)
    {
    }
}
