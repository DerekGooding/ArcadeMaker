namespace Exp.Spans;

internal class OpeningBracketSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "(";

    internal OpeningBracketSpan() : base("(")
    {
    }
}
