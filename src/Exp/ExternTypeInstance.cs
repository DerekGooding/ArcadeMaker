using Exp.Spans;

namespace Exp;

public class ExternTypeInstance : Instance
{
    public object ExternInstance { get; }

    public ExternTypeInstance(object inst) : base(ClassDefSpan.ExternTypeValueDef)
    {
        ArgumentNullException.ThrowIfNull(inst);

        this.ExternInstance = inst;

        // load properties
        foreach (var prop in inst.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
        {
            var var = new ExternPropertyVar(inst, prop);
            Vars.Add(var);
        }
    }
}