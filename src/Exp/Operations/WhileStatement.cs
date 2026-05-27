using Exp.Spans;

namespace Exp.Operations;

internal class WhileStatement(WhileConditionSpan ctx, IReadingOperation cond, IOperation[] innerOps, Variable counter) : ILoopStatement, IConditionalStatement
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    public ConditionSpan ConditionSpan => ctx ?? throw new ArgumentNullException();
    public ILoopContext LoopContext => ctx ?? throw new ArgumentNullException();
    public IReadingOperation ConditionReading => cond ?? throw new ArgumentNullException();
    public IOperation[] InnerOperations => innerOps;

    public void Run()
    {
        IValue counterNum = 0d.ToExp();
        counter?.SetSkippingConstant(counterNum);

        while (cond.Read().Bool)
        {
            foreach (var op in InnerOperations)
            {
                op.Make();
                if (LoopContext.Break)
                {
                    LoopContext.Break = false;
                    goto break_while;
                }
                if (LoopContext.Continue)
                {
                    LoopContext.Continue = false;
                    goto continue_while;
                }
            }

        continue_while:
            if (counter != null)
            {
                counterNum = (counterNum.Number + 1).ToExp();
                counter.SetSkippingConstant(counterNum);
            }
        }
    break_while: { }
    }
}
