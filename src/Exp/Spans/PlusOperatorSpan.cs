namespace Exp.Spans;

internal class PlusOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "+";
    internal override bool TwoSides { get; } = true;

    internal PlusOperatorSpan() : base(Symbol)
    {
    }

    internal override IValue Result(IValue left, IValue right) => GetResult(left, right, this);

    internal static IValue GetResult(IValue left, IValue right, Span thrower)
    {
        if (left != null || right != null)
        {
            if (left == null && right != null && right.IsInst)
                return (null as object) + right.Inst;
            else if (left != null && left.IsInst && right == null)
                return left.Inst + (null as object);

            if (left != null && right != null)
            {
                if (left.IsNumber && right.IsNumber)
                    return (left.Number + right.Number).ToExp();
                else if (left.IsChar && right.IsChar)
                    return ((double)left.Char + right.Char).ToExp();
                else if (left.IsInst)
                    return left.Inst + right;
                else if (right.IsInst)
                    return left + right.Inst;
            }
        }

        OperationFailed($"Cannot add {TypeOrNull(right)} to {TypeOrNull(left)}.", thrower);
        return null;
    }
}
