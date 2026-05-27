using Exp.Spans;

namespace Exp.Operations;

internal class CatchStatement(IOperation[] body, IReadingOperation when, CatchWordSpan word) : IOperationWithInnerSource
{
    public IContext Context => word;
    public bool IsRunning { get; set; }
    public IOperation[] InnerOperations => body;
    internal IReadingOperation When => when;
    internal CatchWordSpan Word => word;

    public void Run()
    {
        foreach (var op in InnerOperations)
            op.Make();
    }
}
