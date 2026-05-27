namespace Exp.Spans;

internal class SetMinusOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "-=";

    internal SetMinusOperatorSpan() : base(Symbol)
    {
    }

    internal override bool TwoSides => true;
    internal override bool Action => true;

    internal override IValue Result(IValue left, IValue right) => MinusOperatorSpan.GetResult(left, right, this);
}
