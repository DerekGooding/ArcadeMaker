using Exp.Spans;

namespace Exp.Operations;

internal class ForeachStatement(ForEachLoopSpan ctx, Variable var, IReadingOperation readingOperation, IOperation[] innerOps, Variable counter) : ILoopStatement
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    internal Variable Var => var ?? throw new ArgumentNullException();
    internal IReadingOperation ReadingOperation => readingOperation ?? throw new ArgumentNullException();
    public IOperation[] InnerOperations => innerOps;
    public ILoopContext LoopContext => ctx ?? throw new ArgumentNullException();

    public void Run()
    {
        IValue counterNum = 0d.ToExp();
        counter?.SetSkippingConstant(counterNum);
        var iter = ReadingOperation.Read()?.Inst;
        if (iter?.GetBasearray() is Instance { IsArray: true } array)
        {
            foreach (var item in array.ArrayValues)
            {
                Var.SetSkippingConstant(item);
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
            }
        break_fe: { }
        }
        else if (iter?.def.HasTag(AttributeDefSpan.IteratableAttr) == true)
        {
            var hasNextFunc = iter.def.Funcs.First(f => f.Name == ForEachLoopSpan.IteratableFunc_HasNext);
            var nextFunc = iter.def.Funcs.First(f => f.Name == ForEachLoopSpan.IteratableFunc_Next);
            var resetFunc = iter.def.Funcs.First(f => f.Name == ForEachLoopSpan.IteratableFunc_Reset);

            Interpreter.Activated.FuncCall(iter, resetFunc, null, out var _, []);
            while (Interpreter.Activated.FuncCall(iter, hasNextFunc, null, out var _, [])?.Bool == true)
            {
                Var.SetSkippingConstant(Interpreter.Activated.FuncCall(iter, nextFunc, null, out var _, []));
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
            }
        break_fe:;
        }
        else
        {
            var err = iter == null ? "The object to iterate over was null" : $"The object to iterate over was not an array, basearray-setter or an instance of a class containg a tag of attribute '{AttributeDefSpan.IteratableAttr.GetExpTypeName(false)}', but it was of type '{Extensions.GetExpTypeName(iter, false)}'";
            Interpreter.Activated.ThrowRuntime($"Foreach loop failed: {err}.", RuntimeException.INVALID_OPERATION, ctx);
        }
    }
}
