namespace Exp;

public class CustomVariable(string name, Func<IValue?> getter, Action<IValue?>? setter, bool prvt = false) : Variable(name, null, null, prvt, setter == null)
{
    public override IValue? Value { get => Getter?.Invoke(); set => Setter?.Invoke(value); }
    public Func<IValue?>? Getter { get; set; } = getter;
    public Action<IValue?>? Setter { get; set; } = setter;
}