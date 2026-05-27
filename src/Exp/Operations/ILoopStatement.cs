using Exp.Spans;

namespace Exp.Operations;

internal interface ILoopStatement : IOperationWithInnerSource
{
    ILoopContext LoopContext { get; }
}
