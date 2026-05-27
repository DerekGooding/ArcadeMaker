using Exp.Spans;

namespace Exp.Operations;

internal class LenofReadingOperation(IReadingOperation arrayReading, LenofWordSpan word) : IReadingOperation
{
    internal IReadingOperation ArrayReading => arrayReading ?? throw new ArgumentNullException();

    public IValue Read()
    {
        var array = ArrayReading.Read().Inst;
        if (array != null)
        {
            if (array.IsArray == true)
                return new NumberValue(array.ArrayValues.Length);
            else
                Interpreter.Activated.ThrowRuntime($"lenof operation failed: {Extensions.GetExpTypeName(array, false)} is not an array.", RuntimeException.INVALID_OPERATION, word);
        }
        else
            Interpreter.Activated.ThrowRuntime($"lenof operation failed: the given value was null.", RuntimeException.INVALID_OPERATION, word);
        throw null;
    }
}
