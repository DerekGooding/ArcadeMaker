using Exp.Spans;

namespace Exp.Operations;

internal class TryStatement(TryWordSpan ctx, IOperation[] body, CatchStatement catc, FinallyStatement finaly) : IOperationWithInnerSource
{
    public IContext Context => ctx;
    public bool IsRunning { get; set; }
    public IOperation[] InnerOperations => body;

    public void Run()
    {
        try
        {
            foreach (var op in InnerOperations)
                op.Make();
        }
        catch (RuntimeException ex) //when (catc != null && (catc.When == null || (catc.When.Read() ?? OnWhenReturnsNull()).Bool)) // when returns null handling is not working this way
        {
            if (catc == null)
                throw;
            if (catc.When != null && !(catc.When.Read() ?? OnWhenReturnsNull()).Bool)
                throw;

            catc.Word.Vars[0].Value = ex.ex;
            ((IOperation)catc).Make();
        }
        finally
        {
            ((IOperation)finaly)?.Make();
        }

        IValue OnWhenReturnsNull()
        {
            Interpreter.Activated.ThrowRuntime("The 'when' condition of a catch statement returned null, which is not allowed.", RuntimeException.INVALID_OPERATION, catc.Word.When.Condition.FirstOrDefault());
            throw null;
        }
    }
}
