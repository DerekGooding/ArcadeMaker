namespace Exp.Spans;

internal class DivideOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "/";
    internal override bool TwoSides { get; } = true;

    internal DivideOperatorSpan() : base(Symbol)
    {
    }

    internal override IValue Result(IValue left, IValue right)
    {
        if (left != null && right != null && left.IsNumber && right.IsNumber)
        {
            if (right.Number == 0)
                Interpreter.Activated.ThrowRuntime("Divide by zero.", RuntimeException.DIVIDE_BY_ZERO);
            return (left.Number / right.Number).ToExp();
        }
        else
            OperationFailed($"Cannot divide {TypeOrNull(left)} by {TypeOrNull(right)}.");
        throw null;
    }
}
