using Exp.Operations;

namespace Exp.Spans;

internal class AndOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "&";
    internal override bool TwoSides { get; } = true;

    internal AndOperatorSpan() : base(Symbol)
    {
    }

    internal override IValue Result(IReadingOperation left, IReadingOperation right) =>
        /*
if (left.IsBool && right.IsBool)
return left.Bool && right.Bool;
else
OperationFailed($"The {Text} symbol must appear between 2 booleans.");
return null;*/
        (left?.Read()?.Bool == true && right?.Read()?.Bool == true).ToExp();

    internal override BoolValue Result(IValue left, IValue right) => left.Bool && right.Bool;
}
