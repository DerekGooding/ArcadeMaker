using Exp.Operations;

namespace Exp.Spans;

internal class SetSymbolSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "=";

    internal SetSymbolSpan() : base(Symbol)
    {
    }

    internal override bool Action { get; } = true;
    internal override bool TwoSides { get; } = true;

    internal override IValue Result(IValue left, IValue right) => right;

    internal override IValue Result(IReadingOperation left, IReadingOperation right) => Result(null, right.Read());
}
