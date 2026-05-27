namespace Exp.Spans;

// an item that has its own vars list
public interface IVarSystem
{
    internal List<Variable> Vars { get; }
    internal IVarSystem Parent { get; set; }

    internal IEnumerable<IValue> BackupValues() => Vars.Map(v => v.Value.Pass());

    internal void RestoreValues(IEnumerable<IValue> values)
    {
        int i = 0;
        foreach (var val in values)
            Vars[i++].SetSkippingConstant(val);
    }
}
