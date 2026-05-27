namespace Exp.Spans;

internal class NamespaceWordSpan : WordSpan, IKeyword
{
    public static string Keyword { get; } = "namespace";
    internal string Namespace { get; }

    internal NamespaceWordSpan(string ns) : base(Keyword)
    {
        Namespace = ns;
    }

    internal override string FullText => $"{Keyword} {Namespace}:";
}
