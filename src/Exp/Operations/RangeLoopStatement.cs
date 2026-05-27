using Exp.Spans;

namespace Exp.Operations;

internal class RangeLoopStatement(RangeLoopSpan ctx, Variable var, IReadingOperation fromReadingOperation, IReadingOperation toReadingOperation, IOperation[] innerOps, Variable counter) : ILoopStatement
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    internal Variable Var => var ?? throw new ArgumentNullException();
    internal IReadingOperation ToReadingOperation => toReadingOperation ?? throw new ArgumentNullException();
    public IOperation[] InnerOperations => innerOps;
    public ILoopContext LoopContext => ctx ?? throw new ArgumentNullException();

    public void Run()
    {
        var from = fromReadingOperation?.Read().Number ?? 0d;
        var to = ToReadingOperation.Read().Number;
        var toIsHigh = to > from;

        IValue counterNum = 0d.ToExp();
        counter?.SetSkippingConstant(counterNum);
        for (var.SetSkippingConstant(from.ToExp()); toIsHigh ? var.Value.Number < to : var.Value.Number > to; var.Value = toIsHigh ? (var.Value.Number + 1).ToExp() : (var.Value.Number - 1).ToExp())
        {
            foreach (var op in InnerOperations)
            {
                op.Make();
                if (LoopContext.Break)
                {
                    LoopContext.Break = false;
                    goto break_fe;
                }
                if (LoopContext.Continue)
                {
                    LoopContext.Continue = false;
                    goto continue_fe;
                }
            }

        continue_fe:
            if (counter != null)
            {
                counterNum = (counterNum.Number + 1).ToExp();
                counter.SetSkippingConstant(counterNum);
            }

            if (var.Value?.IsNumber != true)
                Interpreter.Activated.ThrowRuntime($"The value of '{var.Name}' must be a number, because it is used by the {RangeLoopSpan.ItemName} (The value was {var.Value.GetExpTypeName(true)}).", RuntimeException.INVALID_ARGUMENT, ctx);
        }
    break_fe: { }
    }
}
