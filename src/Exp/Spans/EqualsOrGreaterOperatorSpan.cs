namespace Exp.Spans;

internal class EqualsOrGreaterOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = ">=";
    internal override bool TwoSides { get; } = true;

    internal EqualsOrGreaterOperatorSpan() : base(Symbol)
    {
    }

    internal override BoolValue Result(IValue left, IValue right)
    {
        if (left != null && right != null)
        {
            if (left.IsNumber && right.IsNumber)
                return left.Number >= right.Number;
            else if (left.IsChar && right.IsChar)
                return left.Char >= right.Char;
            else if (left.IsNumber && right.IsChar)
                return left.Number >= right.Char;
            else if (left.IsChar && right.IsNumber)
                return left.Char >= right.Number;
        }
        OperationFailed($"Cannot compare {TypeOrNull(left)} to {TypeOrNull(right)}.");
        throw null;
    }
}
