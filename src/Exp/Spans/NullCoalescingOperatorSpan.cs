namespace Exp.Spans;

internal class NullCoalescingOperatorSpan : OperatorSpan, ISymbol
{
    public static string Symbol { get; } = "??";
    internal override bool TwoSides { get; } = true;

    internal NullCoalescingOperatorSpan() : base(Symbol)
    {
    }

    internal Instance NullCoalsingEx { get; set; }

    internal override IValue Result(IValue left, IValue right)
    {
        IValue value = left ?? right;
        if (value is ThrowWordSpan)
            Interpreter.Activated.ThrowRuntime(NullCoalsingEx.Vars[0].Value.ToString(), NullCoalsingEx.Vars[1].Value.ToString(), this);
        return value;
    }
}
