using Exp.Spans;

namespace Exp.Operations;

internal class Throwing(IReadingOperation exread) : IOperation, IReadingOperation
{
    public void Make() => Read();

    public IValue Read()
    {
        var ex = exread.Read()?.Inst;

        if (ex == null)
            Interpreter.Activated.ThrowRuntime("The value to throw was null.", RuntimeException.NULL_REFERENCE);
        if (ex.def != ClassDefSpan.ExpExceptionDef)
            Interpreter.Activated.ThrowRuntime($"Only instances of type {ClassDefSpan.ExpExceptionDef.GetExpTypeName(false)} can be thrown, but the given value was of type {ex.def.GetExpTypeName(false)}.", RuntimeException.INVALID_ARGUMENT);

        Interpreter.Activated.ThrowRuntime(ex);
        throw null;
    }
}
