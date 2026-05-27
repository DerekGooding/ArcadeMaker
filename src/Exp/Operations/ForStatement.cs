using Exp.Spans;

namespace Exp.Operations;

internal class ForStatement(ForLoopSpan ctx, IOperation init, IReadingOperation cond, IOperation step, IOperation[] innerOps, Variable counter) : ILoopStatement, IConditionalStatement
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    public ConditionSpan ConditionSpan => ctx ?? throw new ArgumentNullException();
    public ILoopContext LoopContext => ctx ?? throw new ArgumentNullException();
    internal IOperation Init => init;
    public IReadingOperation ConditionReading => cond ?? throw new ArgumentNullException();
    internal IOperation Step => step;
    public IOperation[] InnerOperations => innerOps ?? throw new ArgumentNullException();

    public void Run()
    {
        // set counter
        IValue counterNum = 0d.ToExp();
        counter?.SetSkippingConstant(counterNum);

        for (init?.Make(); cond.Read().Bool; step?.Make())
        {
            foreach (var op in InnerOperations)
            {
                op.Make();

                if (LoopContext.Break)
                {
                    LoopContext.Break = false;
                    goto break_for;
                }
                if (LoopContext.Continue)
                {
                    LoopContext.Continue = false;
                    goto continue_for;
                }
            }

            if (counter != null)
            {
                counterNum = (counterNum.Number + 1).ToExp();
                counter.SetSkippingConstant(counterNum);
            }
        continue_for:;
        }
    break_for:;
    }
}
