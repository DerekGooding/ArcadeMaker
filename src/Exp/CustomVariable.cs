namespace Exp;

public class CustomVariable : Variable
{
    public override IValue? Value { get => Getter?.Invoke(); set => Setter?.Invoke(value); }
    public Func<IValue?>? Getter { get; set; }
    public Action<IValue?>? Setter { get; set; }

    public CustomVariable(string name, Func<IValue?> getter, Action<IValue?>? setter, bool prvt = false) : base(name, null, null, prvt, setter == null)
    {
        this.Getter = getter;
        this.Setter = setter;
    }
}