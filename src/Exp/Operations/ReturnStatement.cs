using Exp.Spans;

namespace Exp.Operations;

internal class ReturnStatement(FuncDefSpan func, IReadingOperation readingOperation, ReturnWordSpan rword) : IOperation
{
    internal FuncDefSpan Func => func ?? throw new ArgumentNullException();

    public void Make()
    {
        // break any loop until getting to the function, and return
        Span span = rword;
        while (span != null)
        {
            if (span is ILoopContext loopctx)
            {
                loopctx.Break = true;
            }
            span = span.Container;
        }
        func.Returns = readingOperation.Read();
        func.Return = true;
        Interpreter.Activated.toReturn = true;
    }
}
