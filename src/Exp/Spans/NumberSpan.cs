namespace Exp.Spans;

internal class NumberSpan : Span
{
    internal double Number;

    internal NumberSpan(double num) : base(num.ToString())
    {
        this.Number = num;
    }

    internal NumberSpan(string text) : base(text)
    {
        this.Number = Convert.ToDouble(text);
    }
}
