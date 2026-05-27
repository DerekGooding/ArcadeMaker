using Exp.Spans;

namespace Exp.Operations;

internal class ArrayReadingOperation(IReadingOperation[] readings) : IReadingOperation
{
    internal IReadingOperation[] Readings => readings ?? throw new ArgumentNullException();

    public IValue Read() => new Instance(ClassDefSpan.ExpArrayDef, readings.Select(r => r.Read()).ToArray());
}
