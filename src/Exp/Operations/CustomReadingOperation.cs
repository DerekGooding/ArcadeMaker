namespace Exp.Operations;

internal class CustomReadingOperation<T>(Func<T> func) : IReadingOperation where T : IValue
{
    public IValue Read() => func();
}