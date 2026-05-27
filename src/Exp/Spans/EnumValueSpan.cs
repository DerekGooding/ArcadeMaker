namespace Exp.Spans;

internal class EnumValueSpan : WordSpan, ICanSetAttr, IExpItem
{
    public static string ItemName { get; } = "enum value";
    internal string Name { get; }
    internal double Value { get; set; }
    internal bool CustomValue { get; }
    public List<Span[]> TagsCode { get; set; } = [];
    public Instance[] AttrInfo { get; set; }

    internal EnumValueSpan(string name, double value, bool customValue) : base(name)
    {
        this.Name = name;
        this.Value = value;
        this.CustomValue = customValue;
    }

    internal override string FullText => Name + (CustomValue ? $" = {Value}" : "");
}
