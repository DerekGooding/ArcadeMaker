namespace Exp.Spans;

public interface IDefinition
{
    string Name { get; }
    string? Namespace { get; set; }
    public string FullName => (Namespace == null ? "" : (Namespace + NamespaceSpecificationSpan.Symbol)) + Name;
}
