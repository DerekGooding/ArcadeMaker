namespace Exp.Spans;

internal class MinusOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "-";
    internal override bool TwoSides { get; } = true;

    internal MinusOperatorSpan() : base(Symbol)
    {
    }

    internal override IValue Result(IValue left, IValue right) => GetResult(left, right, this);

    internal static IValue GetResult(IValue left, IValue right, Span throwing)
    {
        if (left != null && right != null)
        {
            if (left.IsNumber && right.IsNumber)
                return (left.Number - right.Number).ToExp();
            else if (left.IsChar && right.IsChar)
                return ((char)(left.Char - right.Char)).ToExp();
            else if (left.IsNumber && right.IsChar)
                return ((char)(left.Number - right.Char)).ToExp();
            else if (left.IsChar && right.IsNumber)
                return ((char)(left.Char - right.Number)).ToExp();
        }
        OperationFailed($"Cannot subtract {TypeOrNull(right)} from {TypeOrNull(left)}.", throwing);
        throw null;
    }
}
