using Exp.Spans;

namespace Exp.Operations;

internal class OperatorResultOperation(OperatorSpan opertor, IReadingOperation left, IReadingOperation right) : IReadingOperation
{
    public IValue Read() => opertor.Result(left, right);
}
