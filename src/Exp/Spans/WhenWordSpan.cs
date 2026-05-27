namespace Exp.Spans;

internal class WhenWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "when";
    internal Span[] Condition { get; }

    internal WhenWordSpan(Span[] condition) : base(Keyword)
    {
        this.Condition = condition;
        SetContainer(condition, Container);
    }

    internal override string FullText => $"{Keyword}\n{{\n\t{Condition.ToString(" ")}\n}}";
}
