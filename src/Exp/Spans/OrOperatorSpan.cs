using Exp.Operations;

namespace Exp.Spans;

internal class OrOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "|";
    internal override bool TwoSides { get; } = true;

    internal OrOperatorSpan() : base(Symbol)
    {
    }

    internal override BoolValue Result(IReadingOperation left, IReadingOperation right)
    {
        /*
        if (left.IsBool && right.IsBool)
            return left.Bool && right.Bool;
        else
            OperationFailed($"The {Text} symbol must appear between 2 booleans.");
        return null;*/
        return left?.Read()?.Bool == true || right?.Read()?.Bool == true;
    }

    internal override BoolValue Result(IValue left, IValue right) => left.Bool || right.Bool;
}
