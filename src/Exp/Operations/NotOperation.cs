namespace Exp.Operations;

internal class NotOperation(IReadingOperation readingOperation) : IReadingOperation
{
    internal IReadingOperation ReadingOperation => readingOperation ?? throw new ArgumentNullException();

    public IValue Read() => new BoolValue(!ReadingOperation.Read().Bool);
}
