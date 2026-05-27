namespace Exp.Operations;

internal class ReadingOperation(IValue value) : IReadingOperation
{
    internal IValue Value => value;

    public IValue Read() => Value;
}
