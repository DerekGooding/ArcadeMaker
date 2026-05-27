using Exp.Spans;

namespace Exp.Operations;

internal interface IOperationWithInnerSource : IOperation
{
    IOperation[] InnerOperations { get; }
    IContext Context { get; }
    bool IsRunning { get; set; }

    void Run();

    void IOperation.Make()
    {
        bool wasRunning = IsRunning;
        IsRunning = true;

        IVarSystem thisVs = Context;
        var backup = wasRunning ? thisVs.BackupValues() : null;

        try
        {
            Run();
        }
        finally
        {
            if (backup != null)
                thisVs.RestoreValues(backup);
            IsRunning = wasRunning;
        }
    }
}
