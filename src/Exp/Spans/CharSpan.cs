namespace Exp.Spans;

internal class CharSpan : Span
{
    internal override string FullText => $"'{Text.Replace("\n", @"\n")}'";

    internal CharSpan(char c) : base("" + c)
    {
    }
}
