namespace Exp.Spans;

internal class ClosingBracketSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = ")";

    internal ClosingBracketSpan() : base(Symbol)
    {
    }
}
