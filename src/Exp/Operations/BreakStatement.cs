using Exp.Spans;

namespace Exp.Operations;

internal class BreakStatement(BreakWordSpan bword, ILoopContext loop) : IOperation
{
    internal ILoopContext Loop => loop ?? throw new ArgumentNullException();

    public void Make()
    {
        // break any loop until the given one
        Span span = bword;
        while (true)
        {
            if (span is ILoopContext loopctx)
            {
                loopctx.Break = true;

                if (loopctx == Loop)
                    return;
            }
            else if (span == null)
                throw new Exception("The given loop is not in the stack.");
            span = span.Container;
        }
    }
}
