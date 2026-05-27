namespace Exp;

public class TypeVariable(string varName, IValue? initVal, Func<IValue?, bool> checker, string typeName, bool prvt = false, bool constant = false, bool skipCheckOnInit = false) : Variable(varName, initVal, null, prvt, constant)
{
    public override IValue? Value
    {
        get => base.Value;
        set
        {
            if (skipCheckOnInit || checker == null || checker(value))
                base.Value = value;
            else
                Interpreter.Activated.ThrowRuntime($"The value of '{base.Name}' must be of type {typeName} (Given value was {value?.TypeName ?? "null"}).", RuntimeException.INVALID_OPERATION);
            skipCheckOnInit = false;
        }
    }
}
