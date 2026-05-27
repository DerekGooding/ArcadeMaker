namespace Exp.Spans;

internal class NotEqualsOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "!=";
    internal override bool TwoSides { get; } = true;

    internal NotEqualsOperatorSpan() : base(Symbol)
    {
    }

    internal override BoolValue Result(IValue left, IValue right) => !((IValue)EqualsOperatorSpan.GetResult(left, right)).Bool;
}
