namespace Exp.Spans;

public abstract class Span
{
    public ScriptDocument Document { get; set; }
    public int DocumentLocation { get; set; }
    internal string Text { get; }
    internal virtual Span Container { get; set; }

    internal IVarSystem GetVS() => Container as IVarSystem ?? Interpreter.Activated;

    internal virtual string FullText => Text;

    internal Span(string text) => Text = text;

    internal void SetContainer(Span[] spans, Span? container = null)
    {
        ArgumentNullException.ThrowIfNull(spans);
        container ??= this;

        foreach (var span in spans)
        {
            span.Container = container;
            if (container is IContext parent && span is IContext vs)
                vs.Parent = parent;
        }
    }

    public override string ToString() => Text;
}
