namespace Exp.Spans;

internal class NumberSpan : Span
{
    internal double Number;

    internal NumberSpan(double num) : base(num.ToString())
    {
        Number = num;
    }

    internal NumberSpan(string text) : base(text)
    {
        Number = Convert.ToDouble(text);
    }
}
