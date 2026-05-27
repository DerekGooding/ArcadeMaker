namespace Exp.Operations;

internal class ConstValueReadingOperation(IValue value) : IReadingOperation
{
    public IValue Read() => value;

    internal static IReadingOperation For(IValue value) => new ConstValueReadingOperation(value);
}
