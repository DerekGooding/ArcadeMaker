using Exp.Spans;

namespace Exp.Operations;

internal class ElseStatement(ElseConditionSpan ctx, IOperation[] innerOps) : IOperationWithInnerSource
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    public IOperation[] InnerOperations => innerOps;

    public void Run()
    {
        foreach (var op in InnerOperations) op.Make();
    }
}
