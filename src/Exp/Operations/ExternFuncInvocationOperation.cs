using Exp.Spans;

namespace Exp.Operations;

internal class ExternFuncInvocationOperation(FuncDefSpan invoker, ExternFunc externFunc) : IReadingOperation
{
    public IValue Read() => externFunc.Func.Invoke(invoker.Parent as Instance, invoker.ParamVariables.Map(v => v.Value).ToArray());
}
