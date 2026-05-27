namespace Exp.Spans;

internal class SetPlusOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "+=";

    internal SetPlusOperatorSpan() : base(Symbol)
    {
    }

    internal override bool TwoSides => true;
    internal override bool Action => true;

    internal override IValue Result(IValue left, IValue right) => PlusOperatorSpan.GetResult(left, right, this);
}
