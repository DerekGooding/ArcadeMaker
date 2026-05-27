namespace Exp.Spans;

internal class StringSpan : Span
{
    private readonly bool escaped;

    internal override string FullText => (escaped ? "@" : "") + "\"" + Text + "\"";

    internal StringSpan(string text, bool escaped = false) : base(text) => this.escaped = escaped;
}
