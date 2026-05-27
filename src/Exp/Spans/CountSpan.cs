namespace Exp.Spans;

internal class CountSpan : Span
{
    internal int Count { get; }

    internal CountSpan(int count) : base(".." + count)
    {
        Count = count;
    }
}
