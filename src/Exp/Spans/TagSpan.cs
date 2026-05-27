namespace Exp.Spans;

internal class TagSpan : WordSpan, IExpItem
{
    public static string ItemName { get; } = "property tag";
    internal Span[] Code { get; }

    internal TagSpan(string name, Span[] code) : base(name) => this.Code = code;

    internal override string FullText => "@" + Text;
}