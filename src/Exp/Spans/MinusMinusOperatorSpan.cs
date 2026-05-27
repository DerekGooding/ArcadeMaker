namespace Exp.Spans;

internal class MinusMinusOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "--";

    internal MinusMinusOperatorSpan() : base(Symbol)
    {
    }

    internal override bool TwoSides => false;
    internal override bool Action => true;

    internal override IValue Result(IValue left, IValue right = null)
    {
        if (left == null)
            OperationFailed("Cannot apply {Symbol} on null.");
        return (left.Number - 1).ToExp();
    }
}
