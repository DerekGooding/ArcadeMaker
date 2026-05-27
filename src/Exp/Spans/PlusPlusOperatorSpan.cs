namespace Exp.Spans;

internal class PlusPlusOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "++";

    internal PlusPlusOperatorSpan() : base(Symbol)
    {
    }

    internal override bool TwoSides => false;
    internal override bool Action => true;

    internal override IValue Result(IValue left, IValue right = null)
    {
        if (left == null)
            OperationFailed("Cannot apply {Symbol} on null.");
        return (left.Number + 1).ToExp();
    }
}
