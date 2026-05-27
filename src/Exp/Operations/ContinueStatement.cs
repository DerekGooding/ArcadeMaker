using Exp.Spans;

namespace Exp.Operations;

internal class ContinueStatement(ContinueWordSpan cword, ILoopContext loop) : IOperation
{
    internal ILoopContext Loop => loop ?? throw new ArgumentNullException();

    public void Make()
    {
        // break any loop until the given one, and continue the given one
        Span span = cword;
        while (true)
        {
            if (span is ILoopContext loopctx)
            {
                if (loopctx == Loop)
                {
                    loopctx.Continue = true;
                    return;
                }
                loopctx.Break = true;
            }
            else if (span == null)
                throw new Exception("The given loop is not in the stack.");
            span = span.Container;
        }
    }
}
