namespace Exp.Spans;

internal class CommentSpan : Span
{
    private readonly bool multiLine;

    internal CommentSpan(string comment, bool multiLine = false) : base(comment) => this.multiLine = multiLine;

    internal override string FullText => multiLine ? ("/*" + Text + "*/") : ("//" + Text);
}
