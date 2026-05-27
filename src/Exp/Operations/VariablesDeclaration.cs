namespace Exp.Operations;

internal class VariablesDeclaration(Dictionary<Variable, IReadingOperation> decs) : IOperation
{
    public void Make()
    {
        foreach (var v in decs)
            v.Key.SetSkippingConstant(v.Value?.Read());
    }
}
