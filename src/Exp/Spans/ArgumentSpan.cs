namespace Exp.Spans;

public class ArgumentSpan : WordSpan, IExpItem
{
    public static string ItemName { get; } = "argument";
    internal string Name { get; }
    internal bool NotNull { get; }

    internal ArgumentSpan(string name, bool notNull = false) : base(name)
    {
        Name = name;
        NotNull = notNull;
    }

    internal override string FullText
    {
        get
        {
            string s = Name;
            if (!NotNull)
                s += "?";
            return s;
        }
    }
}
