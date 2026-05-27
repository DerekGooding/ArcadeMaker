namespace Exp.Spans;

public class NamespaceSpecificationSpan : WordSpan, ISymbol
{
    public static string Symbol { get; } = "::";

    internal NamespaceSpecificationSpan() : base(Symbol)
    {
    }
}
