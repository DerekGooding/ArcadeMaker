namespace Exp.Spans;

internal class EnumDefSpan : WordSpan, IDefinition, ICanSetAttr, IKeyword, IExpItem
{
    public static string Keyword { get; } = "enum";
    public static string ItemName { get; } = "enum";
    public string Name { get; }
    public string Namespace { get; set; }
    internal EnumValueSpan[] Values { get; }
    public List<Span[]> TagsCode { get; set; } = [];
    public Instance[] AttrInfo { get; set; }

    internal EnumDefSpan(string name, EnumValueSpan[] values) : base(Keyword)
    {
        this.Name = name;
        this.Values = values;
    }

    internal override string FullText
    {
        get
        {
            string vals = "";
            int index = 0;
            foreach (var val in Values)
                vals += val.FullText + (++index >= Values.Length ? "" : ",") + "\n";
            return Keyword + $" {Name}\n{{\n\t{vals}\n}}";
        }
    }
}
