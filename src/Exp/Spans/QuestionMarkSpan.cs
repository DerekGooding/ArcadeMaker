namespace Exp.Spans;

internal class QuestionMarkSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "?";

    internal QuestionMarkSpan() : base(Symbol)
    {
    }
}
