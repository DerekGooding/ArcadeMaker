namespace Exp.Spans;

internal class InstInitSpan : WordSpan, IKeyword, IExpItem
{
    public static string Keyword { get; } = "new";
    public static string ItemName { get; } = "new statement";
    internal DefNameSpan DefName { get; }

    internal InstInitSpan(DefNameSpan defName) : base(Keyword)
    {
        ArgumentNullException.ThrowIfNull(defName, nameof(defName));
        this.DefName = defName;
    }

    internal override string FullText
    {
        get => Keyword + " " + DefName;
    }
}
