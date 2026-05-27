using System.Reflection;

namespace Exp;

internal class ExternPropertyVar : Variable
{
    private readonly PropertyInfo pinfo;
    private bool initSetComplete;
    private readonly object inst;

    internal ExternPropertyVar(object inst, PropertyInfo pinfo) : base(pinfo.Name.StartWithLowerCase(), null, null)
    {
        ArgumentNullException.ThrowIfNull(inst);
        ArgumentNullException.ThrowIfNull(pinfo);

        this.pinfo = pinfo;
        this.inst = inst;
    }

    public override IValue? Value
    {
        get => TryGetset(() => Interpreter.CsValToExpVal(pinfo.GetValue(inst)));
        set => TryGetset(() =>
        {
            if (!initSetComplete)
                initSetComplete = true;
            else
                pinfo.SetValue(inst, Interpreter.ExpValToCsVal(value));
            return null;
        });
    }

    private IValue TryGetset(Func<IValue> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            ex = ex.InnerException ?? ex;
            Interpreter.Activated.ThrowRuntime(ex.GetType().ToString() + ": " + ex.Message, RuntimeException.EXTERN_OPERATION_FAILED);
            throw null;
        }
    }
}
