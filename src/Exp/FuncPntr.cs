using Exp.Spans;

namespace Exp;

public readonly struct FuncPntr(FuncDefSpan func, Instance? instance) : IValue
{
    public string TypeName => ValueHelper.tfunc;
    public FuncDefSpan Func => func;
    bool IValue.IsFunc => true;
    FuncPntr IValue.FuncPntr => this;
    public Instance? Instance => instance;

    public IValue? Call(Interpreter interpreter, IEnumerable<IValue?> args)
    {
        return interpreter.FuncCall(instance, func, null, out bool _, args);
    }

    public override string ToString() => func.ToString();
}
