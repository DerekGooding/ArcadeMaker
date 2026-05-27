namespace Exp.Spans;

internal class MultiplyOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "*";
    internal override bool TwoSides { get; } = true;

    internal MultiplyOperatorSpan() : base(Symbol)
    {
    }

    internal override IValue Result(IValue left, IValue right)
    {
        if (left != null && right != null)
        {
            if (left.IsNumber && right.IsNumber)
                return (left.Number * right.Number).ToExp();
            if (left.IsString() && right.IsNumber)
            {
                string str = "", original = Interpreter.ExpStringToString(left.Inst);
                for (var i = 0; i < Math.Floor(right.Number); i++)
                    str += original;
                return str.ToExpString();
            }
        }

        OperationFailed($"Cannot multiply {TypeOrNull(left)} by {TypeOrNull(right)}.");
        throw null;
    }
}
