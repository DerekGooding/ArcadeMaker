using Exp.Spans;

namespace Exp.Operations;

internal class IfStatement(IfConditionSpan ctx, IReadingOperation cond, IOperation[] innerOps) : IConditionalStatement
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    public ConditionSpan ConditionSpan => ctx ?? throw new ArgumentNullException();
    public IReadingOperation ConditionReading => cond ?? throw new ArgumentNullException();
    public IOperation[] InnerOperations { get => innerOps; set => innerOps = value; }
    public ElseStatement Else { get; set; }

    public void Run()
    {
        if (cond.Read().Bool)
            foreach (var op in InnerOperations) op.Make();
        else
            (Else as IOperation)?.Make();
    }
}
