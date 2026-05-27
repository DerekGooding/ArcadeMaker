using Exp.Operations;

namespace Exp.Spans;

internal abstract class OperatorSpan : Span
{
    internal OperatorSpan(string op) : base(op)
    {
    }

    internal abstract IValue Result(IValue left, IValue right);

    internal virtual IValue Result(IReadingOperation left, IReadingOperation right) => Result(left?.Read(), right?.Read());

    internal abstract bool TwoSides { get; }
    internal virtual bool Action { get; } = false;

    protected static string TypeOrNull(IValue obj) => Extensions.GetExpTypeName(obj, true);

    protected static void OperationFailed(string err, Span throwing = null) => Interpreter.Activated.ThrowRuntime(err, RuntimeException.INVALID_OPERATION, throwing);

    protected void OperationFailed(string err) => Interpreter.Activated.ThrowRuntime(err, RuntimeException.INVALID_OPERATION, this);
}
