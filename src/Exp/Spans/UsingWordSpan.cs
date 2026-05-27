namespace Exp.Spans;

internal class UsingWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "using";
    internal string Namespace { get; }

    internal UsingWordSpan(string ns) : base(Keyword)
    {
        Namespace = ns;
    }

    internal override string FullText => $"{Keyword} {Namespace}";
}
